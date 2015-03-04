using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Requests;
using GS.Lib.Network.Sockets.Messages.Responses;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private bool m_ReceivedRestore;

        private void HandleIdentify(SharkResponseMessage p_Message)
        {
            switch (p_Message.As<BasicResponse>().Type)
            {
                case "error":
                {
                    Debug.WriteLine(String.Format("Failed to identify with the Chat Service. Error: {0}", p_Message.As<ErrorResponse<Object>>().Error));
                    break;
                }

                case "success":
                {
                    Debug.WriteLine("Successfully authenticated with the Chat Service.");
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
                    
                    JoinChannels(s_ChannelsToJoin, new Dictionary<string, object> {{ "newUser", true }});
                    SendRestoreLookup();

                    if (!m_CurrentChannels.ContainsKey("broadcastDarkLaunch"))
                        InitiateBroadcastDarkLaunch();

                    // TODO: Do we really need to do this?
                    Library.User.StoreChatIdentity();

                    // CHAT_READY

                    BroadcastMessageToSelf("statusRequest");

                    break;
                }

                default:
                {
                    Debug.WriteLine("Unknown response type received!");
                    break;
                }
            }
        }

        private void SendRestoreLookup()
        {
            if (Library.User.Data == null)
                return;

            m_ReceivedRestore = false;

            m_SocketClient.SendMessage(
                new RestoreLookupRequest(Library.User.Data.ArtistID != 0
                    ? Library.User.Data.ArtistID
                    : Library.User.Data.UserID));
        }
    }
}
