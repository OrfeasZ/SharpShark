﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Requests;
using GS.Lib.Network.Sockets.Messages.Responses;
using GS.Lib.Util;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private const Int64 c_MaxOnlineIdleTime = 900 * 1000;

        private void HandleIdentify(SharkResponseMessage p_Message)
        {
            switch (p_Message.As<BasicResponse>().Type)
            {
                case "error":
                {
                    Trace.WriteLine(String.Format("Failed to identify with the Chat Service. Error: {0}", p_Message.As<ErrorResponse>().Error));
                    Library.DispatchEvent(ClientEvent.AuthenticationFailed, new AuthenticationFailureEvent()
                    {
                        Error = p_Message.As<ErrorResponse>().Error
                    });

                    break;
                }

                case "success":
                {
                    Trace.WriteLine("Successfully authenticated with the Chat Service.");
                    var s_Message = p_Message.As<IdentifyResponse>();

                    UID = s_Message.Success.ID.UID;

                    UpdateUserData();

                    var s_ChannelsToJoin = new List<Object>();

                    var s_GlobalChannel = new Dictionary<String, Object>
                    {
                        { "sub", "global" },
                        { "overwrite_params", false },
                        { "create_when_dne", true },
                        { "try_to_own", false }
                    };

                    s_ChannelsToJoin.Add(s_GlobalChannel);

                    var s_FollowingUsers = Library.User.GetFollowingUsers().Select(p_User => p_User.UserID).ToList();
                    SubscribeToMetaUsers(s_FollowingUsers, false, true);

                    // TODO: Verify Artist support works
                    var s_FollowingArtists = Library.User.GetFollowingArtists().Select(p_User => p_User.UserID).ToList();
                    SubscribeToMetaArtists(s_FollowingArtists, false, true);

                    if (Library.User.Data != null && Library.User.Data.ArtistID != 0)
                        s_ChannelsToJoin.Add("artist:" + Library.User.Data.ArtistID);
                    else if (Library.User.Data != null && Library.User.Data.UserID != 0)
                        s_ChannelsToJoin.Add("user:" + Library.User.Data.UserID);
                    
                    // TODO: Start a timer to force status updates every ~5 minutes.

                    JoinChannels(s_ChannelsToJoin, new Dictionary<string, JToken> { { "newUser", true } });
                    SendRestoreLookup();

                    SetSubscriptionParameters("user:" + Library.User.Data.UserID, new Dictionary<string, object>()
                    {
                        {"unsub_alert", true},
                        {"sub_alert", true},
                        {
                            "owners", new List<Dictionary<String, String>>()
                            {
                                new Dictionary<string, string>()
                                {
                                    {"type", "userid"},
                                    {"name", Library.User.Data.UserID.ToString()}
                                }
                            }
                        }
                    });

                    // TODO: Do we really need to do this?
                    Library.User.StoreChatIdentity();

                    // CHAT_READY

                    BroadcastMessageToSelf("statusRequest");

                    break;
                }

                default:
                {
                    Trace.WriteLine("Unknown response type received!");
                    break;
                }
            }
        }

        private void SendRestoreLookup()
        {
            if (Library.User.Data == null)
                return;

            // NOTE: One would assume that we should be able to create a broadcast after we successfully restore
            // our previous state and performing a master promotion.

            m_SocketClient.SendMessage(
                new RestoreLookupRequest(Library.User.Data.ArtistID != 0
                    ? Library.User.Data.ArtistID
                    : Library.User.Data.UserID), p_Message =>
                    {
                        if (p_Message == null)
                        {
                            Trace.WriteLine("Restore command failed.");
                            return;
                        }

                        var s_Message = p_Message.As<ReturnResponse>();

                        if (s_Message == null || s_Message.Return == null ||
                            s_Message.Return.Value<JArray>("values") == null)
                        {
                            Library.DispatchEvent(ClientEvent.Authenticated, null);
                            return;
                        }

                        var s_Values = s_Message.Return.Value<JArray>("values");

                        if (s_Values.Count == 0)
                        {
                            PromoteSelfToMaster("restore_not_found", OnRestoreFinished);
                            return;
                        }

                        var s_RestoreValue = s_Values[0].ToObject<Dictionary<String, JToken>>();

                        if (s_RestoreValue == null)
                        {
                            PromoteSelfToMaster("restore_not_found", OnRestoreFinished);
                            return;
                        }

                        var s_LocalTime = DateTime.UtcNow.ToUnixTimestampMillis() + Library.TimeDifference;
                        var s_RestoreTime = s_RestoreValue.ContainsKey("time")
                            ? s_RestoreValue["time"].Value<Int64>()
                            : s_LocalTime;

                        if ((!s_RestoreValue.ContainsKey("bcastOwner") || s_RestoreValue["bcastOwner"].Value<int>() == 0) &&
                            ((!s_RestoreValue.ContainsKey("status") || s_RestoreValue["status"].Value<int>() == 0) ||
                             (!s_RestoreValue.ContainsKey("songEx") ||
                              s_RestoreValue["songEx"].ToObject<Dictionary<String, JToken>>() == null) ||
                             s_LocalTime - s_RestoreTime > c_MaxOnlineIdleTime))
                        {
                            PromoteSelfToMaster("last_master_idle_lower", OnRestoreFinished);
                            return;
                        }

                        var s_LastUUID = LoggedInMaster != null ? LoggedInMaster.UUID : null;

                        LoggedInMaster = new MasterStatus()
                        {
                            UUID = null,
                            LastMouseMove = s_RestoreTime,
                            LastUpdate = s_RestoreTime,
                            CurrentBroadcast =
                                s_RestoreValue.ContainsKey("bcast") ? s_RestoreValue["bcast"].Value<String>() : null,
                            
                            IsBroadcasting = s_RestoreValue["bcastOwner"].Value<int>(),
                           
                            CurrentlyPlayingSong =
                                s_RestoreValue.ContainsKey("songEx") &&
                                s_RestoreValue["songEx"].ToObject<Dictionary<String, JToken>>() != null
                                    ? 1
                                    : 0,
                        };

                        if (s_LocalTime - s_RestoreTime > 5 * 60 * 1000 ||
                            s_LocalTime - s_RestoreTime > 10 * 1000 &&
                            (!s_RestoreValue.ContainsKey("remora") || Double.Parse(s_RestoreValue["remora"].Value<String>()) == 0.0))
                            PingMaster(OnRestoreFinished);
                        else if (LastMasterUUID != null)
                            PromoteSelfToMaster("were_last_master", OnRestoreFinished);
                        else
                            Library.DispatchEvent(ClientEvent.Authenticated, null);

                    });
        }

        private void OnRestoreFinished(SharkResponseMessage p_SharkResponseMessage)
        {
            // Restore has finished.
            // We should now be fully authenticated.
            Library.DispatchEvent(ClientEvent.Authenticated, null);
        }
    }
}
