using System;
using System.Collections.Generic;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Models;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private void HandleSubUpdate(SharkEvent p_Event)
        {
            var s_Event = p_Event.As<SubUpdateEvent>();

            if (s_Event.Sub != Library.Chat.GetChatChannel(ActiveBroadcastID) &&
                s_Event.Sub != Library.Chat.GetChatChannel(ActiveBroadcastID, true))
            {
                if (s_Event.Type == "publish")
                    Library.Remora.HandlePublish(s_Event);

                return;
            }

            if (s_Event.Sub == Library.Chat.GetChatChannel(ActiveBroadcastID))
            {
                if (s_Event.Params != null && s_Event.Params.ContainsKey("s"))
                {
                    var s_Data = s_Event.Params["s"].ToObject<PlaybackStatusData>();
                    HandlePlaybackStatusUpdate(s_Data);
                    return;
                }

                if (s_Event.Value != null && s_Event.Value.ContainsKey("type"))
                {
                    if (s_Event.Value["type"].Value<String>() == "vipRequest")
                    {
                        HandleVIPRequest(s_Event.Value["data"]);
                        return;
                    }
                }
            }

            if (s_Event.Sub == Library.Chat.GetChatChannel(ActiveBroadcastID, true))
            {
                if (s_Event.Value != null && s_Event.Value.ContainsKey("type"))
                {
                    if (s_Event.Value["type"].Value<String>() == "activeSongVote")
                    {
                        var s_Vote = s_Event.Value["data"]["vote"].Value<int>();
                        var s_QueueSongID = s_Event.Value["data"]["queueSongID"].Value<Int64>();

                        Library.DispatchEvent(ClientEvent.SongVote,
                            new SongVoteEvent() {CurrentVote = s_Vote, QueueSongID = s_QueueSongID});

                        return;
                    }
                }
            }

            // TODO: Implement
        }

        private void HandlePlaybackStatusUpdate(PlaybackStatusData p_Data)
        {
            if (p_Data == null || p_Data.Active == null || p_Data.Active.Data == null)
            {
                PlayingSongID = 0;
                PlayingSongName = PlayingSongAlbum = PlayingSongArtist = null;
                return;
            }

            var s_LastPlayingSong = PlayingSongID;

            PlayingSongID = p_Data.Active.Data.SongID;
            PlayingSongName = p_Data.Active.Data.SongName;
            PlayingSongAlbum = p_Data.Active.Data.AlbumName;
            PlayingSongArtist = p_Data.Active.Data.ArtistName;

            Library.Queue.SetCurrentPlayingSong(p_Data.Active.QueueSongID);

            if (s_LastPlayingSong != PlayingSongID)
            {
                Library.DispatchEvent(ClientEvent.SongPlaying, new SongPlayingEvent()
                {
                    SongID = PlayingSongID,
                    QueueID = Library.Queue.GetQueueIDForSongID(PlayingSongID),
                    SongName = PlayingSongName
                });
            }
        }

        private void HandleVIPRequest(JToken p_Request)
        {
            var s_Action = p_Request["action"].Value<String>();

            if (s_Action == "approveSuggestion")
            {
                var s_SongID = p_Request["songID"].Value<Int64>();

                // TODO: Compute index.
                var s_QueueIDs = AddSongs(new List<Int64>() { s_SongID }, 9999999);

                if (PlayingSongID == 0)
                    PlaySong(s_SongID, s_QueueIDs[s_SongID]);

                return;
            }
        }
    }
}
