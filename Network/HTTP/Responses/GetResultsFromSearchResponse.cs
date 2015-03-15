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
            public List<ResultData> Songs { get; set; }
            public List<ResultData> Artists { get; set; }
            public List<ResultData> Albums { get; set; }
            public List<ResultData> Videos { get; set; }
            public List<ResultData> Playlists { get; set; }
            public List<ResultData> Users { get; set; }
            public List<ResultData> Events { get; set; }
            public String AssignedVersion { get; set; }
        }

        public bool AskForSuggestion { get; set; }
        public String AssignedVersion { get; set; }
        public String Version { get; set; }
        public SearchResultsResult Result { get; set; }
    }
}
