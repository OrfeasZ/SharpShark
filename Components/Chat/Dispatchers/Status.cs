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
                { "time", DateTime.UtcNow.ToUnixTimestampMillis() + Library.TimeDifference }
            });
        }

        private Dictionary<String, Object> GetCurrentPublicStatus(bool p_FullSong = true)
        {
            StatusSongData s_Song = null;
            Dictionary<String, Object> s_SongEx = null;

            // if (SongPlaying)
            if (Library.Broadcast.PlayingSongID != 0)
            {
                s_Song = new StatusSongData()
                {
                    AlbumID = Library.Broadcast.PlayingAlbumID,
                    AlbumName = Library.Broadcast.PlayingSongAlbum,
                    ArtistID = Library.Broadcast.PlayingArtistID,
                    ArtistName = Library.Broadcast.PlayingSongArtist,
                    CoverArtFilename = null,
                    EstimateDuration = 1000,
                    Flags = 0,
                    SongID = Library.Broadcast.PlayingSongID,
                    SongName = Library.Broadcast.PlayingSongName,
                    TrackNum = 0
                };

                s_SongEx = new Dictionary<string, object>()
                {
                    {
                        "b", new Dictionary<String, Object>()
                        {
                            { "sN", Library.Broadcast.PlayingSongName },
                            { "art", null },
                            { "cID", 0 },
                            { "f", 0 },
                            { "estD", 1000 },
                            { "arID", Library.Broadcast.PlayingArtistID },
                            { "t", 0 },
                            { "arN", Library.Broadcast.PlayingSongArtist },
                            { "tags", null },
                            { "arCl", -1 },
                            { "alID", Library.Broadcast.PlayingAlbumID },
                            { "tk", "" },
                            { "alN", Library.Broadcast.PlayingSongAlbum },
                            { "sID", Library.Broadcast.PlayingSongID },
                            { "uID", 0 }
                        }
                    }
                };
            }

            var s_Status = new Dictionary<string, object>()
            {
                { "status", 1 },
                { "time", DateTime.UtcNow.ToUnixTimestampMillis() + Library.TimeDifference },
                { "songEx", s_SongEx  }
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
