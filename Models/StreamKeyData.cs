using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    public class StreamKeyData : SharkObject
    {
        public String FileID { get; set; }

        public String USecs { get; set; }

        public String FileToken { get; set; }

        [JsonProperty("ts")]
        public UInt32 Timestamp { get; set; }

        public bool IsMobile { get; set; }

        public Int64 SongID { get; set; }

        public String StreamKey { get; set; }

        public UInt32 Expires { get; set; }

        public Int64 StreamServerID { get; set; }

        [JsonProperty("ip")]
        public String IP { get; set; }

        public UInt32 RequestTS { get; set; }
    }
}
