using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    public class BroadcastCreationData : SharkObject
    {
        public BroadcastData Broadcast { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }

        public UInt64 Created { get; set; }

        public bool Success { get; set; }
    }
}
