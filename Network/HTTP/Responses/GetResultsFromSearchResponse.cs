using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Responses
{
    class GetResultsFromSearchResponse : SharkObject
    {
        public bool AskForSuggestion { get; set; }
        public String AssignedVersion { get; set; }
        public String Version { get; set; }
        public Dictionary<String, List<SongResultData>> Result { get; set; }
    }
}
