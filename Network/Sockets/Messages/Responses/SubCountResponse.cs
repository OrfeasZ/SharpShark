using System;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SubCountResponse : SharkMessage
    {
        public String Type { get; set; }

        [JsonProperty("sub_count")]
        public Int64 SubCount { get; set; }

        public SubCountResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
