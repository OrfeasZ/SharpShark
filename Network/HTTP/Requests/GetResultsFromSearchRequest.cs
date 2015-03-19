using System;
using System.Collections.Generic;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetResultsFromSearchRequest : SharkObject
    {
        public Int64 Guts { get; set; }
        public String PpOverride { get; set; }
        public String Query { get; set; }
        public List<String> Type { get; set; }

        public GetResultsFromSearchRequest()
        {
            Type = new List<string>();
        }
    }
}
