using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    internal class MasterStatus
    {
        [JsonProperty("uuid")]
        public String UUID { get; set; }

        public UInt64 LastMouseMove { get; set; }
        
        public UInt64 LastUpdate { get; set; }
        
        public String CurrentBroadcast { get; set; }
        
        public int IsBroadcasting { get; set; }
        
        public int CurrentlyPlayingSong { get; set; }
        
        public String Reason { get; set; }
    }
}
