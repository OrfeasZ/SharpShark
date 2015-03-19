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
                        HandleVIPRequest(s_Event.Value["data"], s_Event);
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

                        var s_Index = Library.Queue.GetInternalIndexForSong(s_QueueSongID);
                        
                        if (s_Index != -1)
                            Library.Queue.CurrentQueue[s_Index].Votes += s_Vote;

                        Library.DispatchEvent(ClientEvent.SongVote,
                            new SongVoteEvent() {VoteChange = s_Vote, QueueSongID = s_QueueSongID, CurrentVotes = s_Index != -1 ? Library.Queue.CurrentQueue[s_Index].Votes : 0});

                        return;
                    }

                    if (s_Event.Value["type"].Value<String>() == "suggestion" && s_Event.Value.ContainsKey("data"))
                    {
                        var s_Data = s_Event.Value["data"].ToObject<Dictionary<String, JToken>>();

                        var s_SongData = s_Data["song"].ToObject<PlaybackStatusData.ActiveBroadcastData>();

                        var s_UserData = s_Event.ID["app_data"].ToObject<ChatUserData>();
                        var s_UserID = Int64.Parse(s_Event.ID["userid"].Value<String>());

                        var s_SongEvent = new SongSuggestionEvent()
                        {
                            SongID = s_SongData.Data.SongID,
                            SongName = s_SongData.Data.SongName,
                            ArtistID = s_SongData.Data.ArtistID,
                            ArtistName = s_SongData.Data.ArtistName,
                            AlbumID = s_SongData.Data.AlbumID,
                            AlbumName = s_SongData.Data.AlbumName,
                            User = s_UserData,
                            UserID = s_UserID
                        };

                        Library.DispatchEvent(ClientEvent.SongSuggestion, s_SongEvent);
                        return;
                    }

                    if (s_Event.Value["type"].Value<String>() == "suggestionRemove" && s_Event.Value.ContainsKey("data"))
                    {
                        var s_Data = s_Event.Value["data"].ToObject<Dictionary<String, JToken>>();

                        var s_SongID = s_Data["songID"].Value<Int64>();

                        var s_UserData = s_Event.ID["app_data"].ToObject<ChatUserData>();
                        var s_UserID = Int64.Parse(s_Event.ID["userid"].Value<String>());

                        var s_SongEvent = new SongSuggestionRemovalEvent()
                        {
                            SongID = s_SongID,
                            User = s_UserData,
                            UserID = s_UserID
                        };

                        Library.DispatchEvent(ClientEvent.SongSuggestionRemoved, s_SongEvent);
                        return;
                    }
                }
            }

            // TODO: Implement
        }

        private void HandlePlaybackStatusUpdate(PlaybackStatusData p_Data)
        {
            var s_LastPlayingSong = PlayingSongID;

            if (p_Data == null || p_Data.Active == null || p_Data.Active.Data == null)
            {
                PlayingSongID = PlayingAlbumID = PlayingArtistID = 0;
                PlayingSongQueueID = 0;
                PlayingSongName = PlayingSongAlbum = PlayingSongArtist = null;
            }
            else
            {
                PlayingSongID = p_Data.Active.Data.SongID;
                PlayingAlbumID = p_Data.Active.Data.AlbumID;
                PlayingArtistID = p_Data.Active.Data.ArtistID;
                PlayingSongQueueID = p_Data.Active.QueueSongID;
                PlayingSongName = p_Data.Active.Data.SongName;
                PlayingSongAlbum = p_Data.Active.Data.AlbumName;
                PlayingSongArtist = p_Data.Active.Data.ArtistName;
            }

            if (s_LastPlayingSong != PlayingSongID)
            {
                Library.DispatchEvent(ClientEvent.SongPlaying, new SongPlayingEvent()
                {
                    SongID = PlayingSongID,
                    QueueID = PlayingSongQueueID,
                    SongName = PlayingSongName,
                    ArtistID = PlayingArtistID,
                    ArtistName = PlayingSongArtist,
                    AlbumID = PlayingAlbumID,
                    AlbumName = PlayingSongAlbum
                });
            }
        }

        private void HandleVIPRequest(JToken p_Request, SubUpdateEvent p_Event)
        {
            var s_Action = p_Request["action"].Value<String>();

            if (s_Action == "approveSuggestion")
            {
                var s_SongID = p_Request["songID"].Value<Int64>();

                var s_UserData = p_Event.ID["app_data"].ToObject<ChatUserData>();
                var s_UserID = Int64.Parse(p_Event.ID["userid"].Value<String>());

                var s_QueueIDs = AddSongs(new List<Int64>() { s_SongID });

                if (PlayingSongID == 0)
                    PlaySong(s_SongID, s_QueueIDs[s_SongID]);

                Library.DispatchEvent(ClientEvent.SongSuggestionApproved, new SongSuggestionApprovalEvent()
                {
                    SongID = s_SongID,
                    User = s_UserData,
                    UserID = s_UserID
                });

                return;
            }
        }
    }
}
