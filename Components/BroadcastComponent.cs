using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Enums;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent : EventsComponent
    {
        public String ActiveBroadcastID { get; internal set; }

        public BroadcastData Data { get; internal set; }

        public BroadcastStatus CurrentBroadcastStatus { get; internal set; }
        public String CurrentBroadcastName { get; internal set; }
        public String CurrentBroadcastDescription { get; internal set; }
        public String CurrentBroadcastPicture { get; internal set; }
        public CategoryTag CurrentBroadcastCategoryTag { get; internal set; }

        public Int64 PlayingSongID { get; internal set; }
        public String PlayingSongName { get; internal set; }
        public String PlayingSongArtist { get; internal set; }
        public String PlayingSongAlbum { get; internal set; }

        public List<Int64> SpecialGuests { get; internal set; }

        public bool ChatEnabled { get; internal set; }

        private Int64 m_QueuedSongs;
        
        internal BroadcastComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            Data = null;
            m_QueuedSongs = 0;
            SpecialGuests = new List<long>();

            CurrentBroadcastStatus = BroadcastStatus.Idle;
            ChatEnabled = true;
            CurrentBroadcastCategoryTag = null;

            SuggestionsEnabled = true;

            m_SuggestionChanges = new List<object>();
        }

        internal override void RegisterEventHandlers()
        {
            Library.Chat.RegisterEventHandler((int) ChatEvent.SetResult, HandleSetResult);
            Library.Chat.RegisterEventHandler((int) ChatEvent.SubUpdate, HandleSubUpdate);
        }

        public List<CategoryTag> GetCategoryTags()
        {
            if (String.IsNullOrWhiteSpace(Library.User.SessionID))
                return new List<CategoryTag>();

            var s_Tags = Library.RequestDispatcher.Dispatch<Object, Dictionary<String, String>>("getTagList", null);

            var s_TagList = new List<CategoryTag>();

            if (s_Tags == null)
                return s_TagList;

            s_TagList.AddRange(s_Tags.Select(p_Pair => new CategoryTag(p_Pair.Key, p_Pair.Value)));

            return s_TagList;
        }

        public BroadcastData GetLastBroadcast()
        {
            if (Library.User.Data == null)
                return null;

            var s_Response = Library.RequestDispatcher.Dispatch<Object, BroadcastData>("getUserLastBroadcast", null);
            return s_Response;
        }

        public void CreateBroadcast(String p_Name, String p_Description, CategoryTag p_Tag)
        {
            if (Library.User.Data == null)
                return;

            SpecialGuests.Clear();
            m_QueuedSongs = 0;
            CurrentBroadcastName = p_Name;
            CurrentBroadcastDescription = p_Description;
            CurrentBroadcastCategoryTag = p_Tag;
            ActiveBroadcastID = null;
            Data = null;

            var s_LastBroadcast = GetLastBroadcast();

            if (s_LastBroadcast != null)
            {
                ActiveBroadcastID = s_LastBroadcast.BroadcastID;
                CurrentBroadcastPicture = null;
                Data = s_LastBroadcast;
            }

            Library.Remora.JoinControlChannels();
        }

        public void DestroyBroadcast()
        {
            SpecialGuests.Clear();
            m_QueuedSongs = 0;
        }

        public Dictionary<Int64, Int64> AddSongs(IEnumerable<Int64> p_SongIDs, int p_Index)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return new Dictionary<long, long>();

            var s_SongIDs = p_SongIDs.ToList();
            var s_QueueSongIDs = new List<Int64>();
            var s_QueueSongData = new Dictionary<Int64, Int64>();

            foreach (var s_SongID in s_SongIDs)
            {
                var s_QueueID = ++m_QueuedSongs;
                s_QueueSongData.Add(s_SongID, s_QueueID);
                s_QueueSongIDs.Add(s_QueueID);
            }

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "addSongs" },
                { "index", p_Index },
                { "songIDs", s_SongIDs },
                { "queueSongIDs", s_QueueSongIDs }
            });

            return s_QueueSongData;
        }

        public void PlaySong(Int64 p_SongID, Int64 p_QueueID, double p_Position = 0.0)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "playSong" },
                { "queueSongID", p_QueueID },
                { "country", Library.User.CountryData },
                { "sourceID", 1 },
                { "position", p_Position },
                { "streamType", 0 },
                {
                    "options", new Dictionary<String, Object>()
                    {
                        {
                            "params", new Dictionary<String, Object>()
                            {
                                { "prefetch", false },
                                { "country", Library.User.CountryData },
                                { "mobile", false },
                                { "type", 0 },
                                { "songID", p_SongID }
                            }
                        },
                        { "fastFetch", false}
                    }
                }
            });
        }

        public void MoveSongs(IEnumerable<Int64> p_QueueIDs, int p_Index)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "moveSongs" },
                { "index", p_Index },
                { "queueSongIDs", p_QueueIDs.ToList() }
            });
        }

        public void DisableMobileCompliance()
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "disableMobileCompliance" }
            });
        }

        public void SeekCurrentSong(double p_Position)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "seek" },
                { "position", p_Position }
            });
        }

        public void AddSpecialGuest(Int64 p_UserID, VIPPermissions p_Permissions)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "addSpecialGuest" },
                { "userID", p_UserID },
                { "permission", p_Permissions }
            });

            lock (SpecialGuests)
            {
                if (!SpecialGuests.Contains(p_UserID))
                    SpecialGuests.Add(p_UserID);
            }
        }

        public void RemoveSpecialGuest(Int64 p_UserID)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "removeSpecialGuest" },
                { "userID", p_UserID }
            });

            lock (SpecialGuests)
                SpecialGuests.Remove(p_UserID);
        }

        public void ApproveSuggestion(Int64 p_SongID)
        {
            if (ActiveBroadcastID == null || CurrentBroadcastStatus != BroadcastStatus.Broadcasting)
                return;

            Library.Remora.Send(new Dictionary<String, Object>
            {
                { "action", "approveSuggestion" },
                { "songID", p_SongID }
            });
        }
    }
}
