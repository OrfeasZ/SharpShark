using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    internal class MasterStatus
    {
        [JsonProperty("uuid")]
        public String UUID { get; set; }

        public Int64 LastMouseMove { get; set; }
        
        public Int64 LastUpdate { get; set; }
        
        public String CurrentBroadcast { get; set; }
        
        public int IsBroadcasting { get; set; }
        
        public int CurrentlyPlayingSong { get; set; }
        
        public String Reason { get; set; }
    }
}
