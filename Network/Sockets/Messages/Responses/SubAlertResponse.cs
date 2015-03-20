using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SubAlertResponse : SharkMessage
    {
        public String Type { get; set; }

        [JsonProperty("sub_alert")]
        public Dictionary<String, JToken> SubAlert { get; set; } 

        public SubAlertResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
