using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SubinfoChangeResponse : SharkMessage
    {
        internal class SubinfoChangeData
        {
            public String Sub { get; set; }

            public Dictionary<String, Object> Params { get; set; }

            [JsonProperty("id")]
            public JToken ID { get; set; }
        }

        [JsonProperty("subinfo_change")]
        public SubinfoChangeData SubinfoChange { get; set; }

        public SubinfoChangeResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
