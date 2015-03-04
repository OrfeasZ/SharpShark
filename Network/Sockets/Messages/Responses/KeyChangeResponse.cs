using System;
using System.Collections.Generic;
using GS.Lib.Models;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class KeyChangeResponse : SharkMessage
    {
        internal class KeyChangeData
        {
            [JsonProperty("userid")]
            public String UserID { get; set; }

            [JsonProperty("artistid")]
            public String ArtistID { get; set; }

            public String Sub { get; set; }

            [JsonProperty("keyvals")]
            public List<KeyValData> KeyVals { get; set; }

            [JsonProperty("id")]
            public Dictionary<String, Object> ID { get; set; } 
        }

        [JsonProperty("key_change")]
        public KeyChangeData KeyChange { get; set; }

        public KeyChangeResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
