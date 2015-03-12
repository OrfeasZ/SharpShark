using System;
using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;
using GS.Lib.Network.Sockets.Messages.Responses;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void BroadcastMessageToSelf(String p_Type)
        {
            BroadcastMessageToSelf(new Dictionary<string, string>(), p_Type);
        }

        private void BroadcastMessageToSelf<T>(T p_Params, String p_Type)
        {
            if (Library.User.Data == null)
                return;

            if (p_Params == null)
            {
                BroadcastMessageToSelf(new Dictionary<string, string>(), p_Type);
                return;
            }

            if (Library.User.Data.ArtistID != 0)
            {
                PublishToChannels(new List<string>() { "artist:" + Library.User.Data.ArtistID }, new Dictionary<String, Object>()
                {
                    { "type", p_Type },
                    { "params", p_Params },
                    { "sessionPart", GetSessionPart() }
                });
            }
            else if (Library.User.Data.UserID != 0)
            {
                PublishToChannels(new List<string>() { "user:" + Library.User.Data.UserID }, new Dictionary<String, Object>()
                {
                    { "type", p_Type },
                    { "params", p_Params },
                    { "sessionPart", GetSessionPart() }
                });
            }
        }

        internal void SubscribeToBroadcastStatuses()
        {
            if (Library.Broadcast.ActiveBroadcastID == null)
                return;

            var s_Request = new SubscribeToMetaSubKeysRequest(new List<SubscriptionKeys>()
            {
                new SubscriptionKeys()
                {
                    Sub = GetChatChannel(Library.Broadcast.ActiveBroadcastID),
                    Keys =
                        new List<string>()
                        {
                            "s",
                            "h",
                            "owners",
                            "n",
                            "t",
                            "owner_subscribed",
                            "i",
                            "d",
                            "tl",
                            "v",
                            "pndOwner",
                            "py",
                            "tags",
                            "qc",
                            "idSuffix",
                            "urls",
                            "pi",
                            "p"
                        }
                },
                new SubscriptionKeys()
                {
                    Sub = GetChatChannel(Library.Broadcast.ActiveBroadcastID, true),
                    Keys =
                        new List<string>()
                        {
                            "c",
                            "banned",
                            "sge",
                            "sg",
                            "sgc",
                            "vp",
                            "idSuffix"
                        }
                },

            });

            m_SocketClient.SendMessage(s_Request, p_Message =>
            {
                var s_Response = p_Message.As<BasicResponse>();

                if (s_Response == null || s_Response.Type != "success")
                {
                    // TODO: Broadcast status subscription failed. Destroy broadcast and notify client.
                    return;
                }

                SubscribeToBroadcast();
            });
        }

        internal void SubscribeToBroadcast()
        {
            if (Library.Broadcast.ActiveBroadcastID == null)
                return;

            Library.Chat.JoinChannels(new List<object>()
            {
                new Dictionary<String, Object>()
                {
                    { "sub", GetChatChannel(Library.Broadcast.ActiveBroadcastID) },
                    { "overwrite_params", false },
                    { "create_when_dne", true }
                },
                new Dictionary<String, Object>()
                {
                    { "sub", GetChatChannel(Library.Broadcast.ActiveBroadcastID, true) },
                    { "overwrite_params", false },
                    { "create_when_dne", false }
                }
            }, p_Callback: p_Message =>
            {
                // Subscription successful.
                // TODO: Dispatch event, start pinging remora.
                Library.Remora.StartPing();
            });
        }

    }
}
