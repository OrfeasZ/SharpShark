using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public class BroadcastComponent : SharkComponent
    {
        internal BroadcastComponent(SharpShark p_Library) 
            : base(p_Library)
        {
        }

        public List<CategoryTag> GetCategoryTags()
        {
            var s_Tags = Library.RequestDispatcher.Dispatch<Dictionary<String, String>, Dictionary<String, String>>("getTagList",
                new Dictionary<string, string>());

            var s_TagList = new List<CategoryTag>();

            if (s_Tags == null)
                return s_TagList;

            s_TagList.AddRange(s_Tags.Select(p_Pair => new CategoryTag(p_Pair.Key, p_Pair.Value)));

            return s_TagList;
        }
    }
}
