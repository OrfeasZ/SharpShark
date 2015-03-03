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
        private void UpdateCurrentStatus(bool p_SendVisibilityFriends = false, UserStatus p_Status = null)
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
            UpdateCurrentStatus(false, new UserStatus() { Status = 0, Time = DateTime.UtcNow.ToUnixTimestamp() * 1000 });
        }

        private UserStatus GetCurrentPublicStatus(bool p_FullSong = true)
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

            var s_Status = new UserStatus()
            {
                Status = 1,
                Time = DateTime.UtcNow.ToUnixTimestamp() * 1000,
                SongEx = null // TODO: Set song data
            };

            if (p_FullSong)
                s_Status.Song = s_Song;

            if (!String.IsNullOrEmpty(Library.Broadcast.CurrentBroadcast))
            {
                s_Status.Bcast = Library.Broadcast.CurrentBroadcast;
                s_Status.BcastName = Library.Broadcast.CurrentBroadcastName;
                s_Status.BcastPic = Library.Broadcast.CurrentBroadcastPicture;
                s_Status.BcastOwner = Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Broadcasting ? 1 : 0;

                if (Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Listening)
                {
                    // TODO: Set owner info.
                    s_Status.BcastOwnerInfo = new object();
                }
            }

            return s_Status;
        }
    }
}
