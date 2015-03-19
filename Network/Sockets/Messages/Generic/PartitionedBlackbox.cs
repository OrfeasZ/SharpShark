using System;

namespace GS.Lib.Network.Sockets.Messages.Generic
{
    internal class PartitionedBlackbox : SharkObject
    {
        public String Type { get; set; }

        public String Source { get; set; }

        public Int64 PartsTotal { get; set; }

        public Int64 PartID { get; set; }

        public bool Initial { get; set; }

        public bool Retried { get; set; }
    }
}
