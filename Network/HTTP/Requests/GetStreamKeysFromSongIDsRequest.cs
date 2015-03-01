using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetStreamKeysFromSongIDsRequest : SharkObject
    {
        public CountryData Country { get; set; }

        public bool Mobile { get; set; }

        public bool Prefetch { get; set; }

        public bool ReturnTS { get; set; }

        public List<Int64> SongIDs { get; set; }

        public Int64 Type { get; set; }

        public GetStreamKeysFromSongIDsRequest()
        {
            Country = new CountryData();
            Mobile = false;
            Prefetch = true;
            ReturnTS = true;
            SongIDs = new List<long>();
        }
    }
}
