using System;
using System.Collections.Generic;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetResultsFromSearchRequest : SharkObject
    {
        public int Guts { get; set; }
        public bool PpOverride { get; set; }
        public String Query { get; set; }
        public List<String> Type { get; set; }

        public GetResultsFromSearchRequest()
        {
            Type = new List<string>();
        }
    }
}
