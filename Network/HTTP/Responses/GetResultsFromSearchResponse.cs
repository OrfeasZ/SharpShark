using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Responses
{
    internal class GetResultsFromSearchResponse : SharkObject
    {
        internal class SearchResultsResult
        {
            // TODO: Check which of these types need fixing
            public List<SongResultData> Songs { get; set; }
            public List<SongResultData> Artists { get; set; }
            public List<SongResultData> Albums { get; set; }
            public List<SongResultData> Videos { get; set; }
            public List<SongResultData> Playlists { get; set; }
            public List<SongResultData> Users { get; set; }
            public List<SongResultData> Events { get; set; }
            public String AssignedVersion { get; set; }
        }

        public bool AskForSuggestion { get; set; }
        public String AssignedVersion { get; set; }
        public String Version { get; set; }
        public SearchResultsResult Result { get; set; }
    }
}
