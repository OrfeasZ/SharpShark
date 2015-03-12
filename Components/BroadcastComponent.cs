using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Enums;
using GS.Lib.Models;
using GS.Lib.Network.HTTP.Requests;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent : EventsComponent
    {
        public String ActiveBroadcastID { get; set; }

        public BroadcastData Data { get; set; }

        public BroadcastStatus CurrentBroadcastStatus { get; set; }
        public String CurrentBroadcastName { get; set; }
        public String CurrentBroadcastDescription { get; set; }
        public String CurrentBroadcastPicture { get; set; }
        public CategoryTag CurrentBroadcastCategoryTag { get; set; }

        public bool ChatEnabled { get; set; }

        private const double c_BroadcasterVersion = 3.91;

        internal BroadcastComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            Data = null;

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
    }
}
