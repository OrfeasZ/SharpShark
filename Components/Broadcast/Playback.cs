using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        public bool IsPlaying { get; set; }

        private Dictionary<String, Object> GetChatVariableStatus()
        {
            // TODO: Implement
            return new Dictionary<string, object>();
        }

        private List<String> GetChatVariableTags()
        {
            var s_Tags = new List<String>() { "bcast" };

            if (CurrentBroadcastCategoryTag != null)
                s_Tags.Add("bcast_genre_" + CurrentBroadcastCategoryTag.Index);

            return s_Tags;
        }
    }
}
