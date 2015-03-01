using System;
using System.Collections.Generic;

namespace GS.Lib.Network.HTTP.Responses
{
    internal class LibrarySongIDsResponse : SharkObject
    {
        public bool FromSongs { get; set; }

        public UInt32 TSModifier { get; set; }

        public List<Int64> SongIDs { get; set; } 
    }
}
