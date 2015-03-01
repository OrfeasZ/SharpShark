using System;

namespace GS.Lib.Models
{
    internal class MasterStatus
    {
        public String UUID { get; set; }
        public UInt32 LastMouseMove { get; set; }
        public UInt32 LastUpdate { get; set; }
        public String CurrentBroadcast { get; set; }
        public bool IsBroadcasting { get; set; }
        public bool CurrentlyPlayingSong { get; set; }
        public String Remora { get; set; }
        public String Reason { get; set; }
    }
}
