using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class PublishResponse : SharkMessage
    {
        internal class PublishData
        {
            public String Type { get; set; }

            public String Destination { get; set; }

            [JsonProperty("destination_type")]
            public String DestinationType { get; set; }

            public Dictionary<String, JToken> Value { get; set; }

            [JsonProperty("id")]
            public Dictionary<String, JToken> ID { get; set; } 
        }

        public PublishData Publish { get; set; }

        public PublishResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
