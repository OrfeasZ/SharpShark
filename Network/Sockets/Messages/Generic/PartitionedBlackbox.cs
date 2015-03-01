using System;

namespace GS.Lib.Network.Sockets.Messages.Generic
{
    internal class PartitionedBlackbox : SharkObject
    {
        public String Type { get; set; }

        public String Source { get; set; }

        public int PartsTotal { get; set; }

        public int PartID { get; set; }

        public bool Initial { get; set; }

        public bool Retried { get; set; }
    }
}
