using System;
using System.Collections.Generic;
using GS.Lib.Enums;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;
using GS.Lib.Util;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        internal void UpdateCurrentStatus(bool p_SendVisibilityFriends = false, Dictionary<String, Object> p_Status = null)
        {
            if (Library.User.Data == null || Library.User.Data.UserID <= 0 || LoggedInMaster == null ||
                LoggedInMaster.UUID != UID)
                return;

            var s_Status = p_Status ?? GetCurrentPublicStatus();

            var s_Key = new Dictionary<String, Object>()
            {
                { "key", "s" },
                { "value", s_Status }
            };

            if (p_SendVisibilityFriends)
            {
                var s_Params = new Dictionary<String, Object>()
                {
                    {"keyvals", new List<Dictionary<String, Object>>() { s_Key }}
                };

                m_SocketClient.SendMessage(new ParameterizedRequest<Dictionary<String, Object>>("set", s_Params));
            }

            // TODO: Implement different readability for offline and invisible statuses.
            s_Key.Add("readable", "global");
            
            var s_SetParams = new Dictionary<String, Object>()
            {
                {"keyvals", new List<Dictionary<String, Object>>() { s_Key }}
            };

            if (Library.User.Data.ArtistID != 0)
                s_SetParams.Add("artistid", Library.User.Data.ArtistID.ToString());
            else
                s_SetParams.Add("userid", Library.User.Data.UserID.ToString());

            m_SocketClient.SendMessage(new ParameterizedRequest<Dictionary<String, Object>>("set", s_SetParams));

            // Broadcast updated status.
            BroadcastMessageToSelf(s_Status, "statusUpdate");
        }

        private void SetStatusOffline()
        {
            UpdateCurrentStatus(false, new Dictionary<string, object>()
            {
                { "status", 0 }, 
                { "time", DateTime.UtcNow.ToUnixTimestamp() * 1000 }
            });
        }

        private Dictionary<String, Object> GetCurrentPublicStatus(bool p_FullSong = true)
        {
            StatusSongData s_Song = null;

            // if (SongPlaying)
            if (true)
            {
                s_Song = null;
            }
            else
            {
                // TODO: Set song data
            }

            var s_Status = new Dictionary<string, object>()
            {
                { "status", 1 },
                { "time", DateTime.UtcNow.ToUnixTimestamp() * 1000 },
                { "songEx", null  } // TODO: Set song data
            };

            if (p_FullSong)
                s_Status.Add("song", s_Song);

            if (!String.IsNullOrEmpty(Library.Broadcast.ActiveBroadcastID))
            {
                s_Status.Add("bcast", Library.Broadcast.ActiveBroadcastID);
                s_Status.Add("bcastName", Library.Broadcast.CurrentBroadcastName);
                s_Status.Add("bcastPic", Library.Broadcast.CurrentBroadcastPicture);
                s_Status.Add("bcastOwner", Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Broadcasting ? 1 : 0);

                if (Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Listening)
                {
                    // TODO: Set owner info.
                    s_Status.Add("bcastOwnerInfo", new object());
                }
            }

            return s_Status;
        }
    }
}
