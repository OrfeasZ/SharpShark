using System;
using System.Collections.Generic;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        public bool SuggestionsEnabled { get; set; }

        private readonly List<Object> m_SuggestionChanges;

        private const int c_MaxSuggestionChanges = 25;

        private Dictionary<String, Object> GetChatVariableSuggestions()
        {
            return new Dictionary<string, object>()
            {
                { "b", GetBlockedSongIDs() },
                { "s", GetSongsForChatServer() }
            };
        }

        private List<Int64> GetBlockedSongIDs()
        {
            // TODO: Implement
            return new List<long>();
        }

        private Dictionary<String, Object> GetSongsForChatServer()
        {
            // TODO: Implement
            return new Dictionary<string, object>();
        }
    }
}
