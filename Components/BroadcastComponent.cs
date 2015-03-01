using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Enums;
using GS.Lib.Models;
using GS.Lib.Network.HTTP.Requests;

namespace GS.Lib.Components
{
    public class BroadcastComponent : SharkComponent
    {
        public String ActiveBroadcastID { get; set; }

        public BroadcastData Data { get; set; }

        public String CurrentBroadcast { get; set; }
        public BroadcastStatus CurrentBroadcastStatus { get; set; }
        public String CurrentBroadcastName { get; set; }
        public String CurrentBroadcastPicture { get; set; }

        internal BroadcastComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            Data = null;

            CurrentBroadcast = null;
            CurrentBroadcastStatus = BroadcastStatus.Idle;
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

        public BroadcastCreationData CreateBroadcast(String p_Name, String p_Description, CategoryTag p_Tag)
        {
            if (Library.User.Data == null)
                return null;

            var s_Request = new CreateBroadcastRequest()
            {
                Name = p_Name,
                Description = p_Description,
                Tag = p_Tag,
                IsRandomName = false
            };

            var s_Response = Library.RequestDispatcher.Dispatch<CreateBroadcastRequest, BroadcastCreationData>(
                    "createBroadcastFromLast", s_Request);

            if (s_Response != null && s_Response.Success)
            {
                ActiveBroadcastID = s_Response.ID;
                Data = s_Response.Broadcast;
            }

            return s_Response;
        }
    }
}
