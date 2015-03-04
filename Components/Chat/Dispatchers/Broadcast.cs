using System;
using System.Collections.Generic;

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

        internal void ActivateBroadcast(bool p_OwnBroadcast = false, bool p_TryToOwn = true, String p_Source = null,
            Object p_Token = null, String p_PendingBroadcastID = null)
        {
            if (!p_OwnBroadcast)
                p_TryToOwn = false;

            Library.Remora.PublishInitialQueue();
            Library.Remora.PublishActiveSong();

            if (p_Source == null)
                p_Source = p_OwnBroadcast ? "startBroadcast" : "joinBroadcast";

            JoinChannels(new List<object>()
            {
                new Dictionary<String, Object>
                {
                    { "sub", GetChatChannel(Library.Broadcast.ActiveBroadcastID) },
                    { "overwrite_params", false },
                    { "create_when_dne", p_TryToOwn },
                    { "try_to_own", p_TryToOwn }
                }, 
                new Dictionary<String, Object>
                {
                    { "sub", GetChatChannel(Library.Broadcast.ActiveBroadcastID, true) },
                    { "overwrite_params", false },
                    { "create_when_dne", p_TryToOwn },
                    { "try_to_own", p_TryToOwn }
                }
            },
            new Dictionary<String, Object>
            {
                { "source", p_Source },
                { "broadcastID", Library.Broadcast.ActiveBroadcastID },
                { "token", p_Token },
                { "isBroadcasting", p_OwnBroadcast },
                { "pendingNewBroadcastID", p_PendingBroadcastID }
            });
        }
    }
}
