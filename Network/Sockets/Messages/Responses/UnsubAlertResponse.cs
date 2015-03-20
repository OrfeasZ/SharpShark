using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class UnsubAlertResponse : SharkMessage
    {
        public String Type { get; set; }

        [JsonProperty("unsub_alert")]
        public Dictionary<String, JToken> UnsubAlert { get; set; }

        public UnsubAlertResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
