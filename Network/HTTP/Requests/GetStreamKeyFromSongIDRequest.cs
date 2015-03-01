using System;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetStreamKeyFromSongIDRequest : SharkObject
    {
        public CountryData Country { get; set; }

        public bool Mobile { get; set; }

        public bool Prefetch { get; set; }

        public bool ReturnTS { get; set; }

        public Int64 SongID { get; set; }

        public Int64 Type { get; set; }

        public GetStreamKeyFromSongIDRequest()
        {
            Country = new CountryData();
            Mobile = false;
            Prefetch = true;
            ReturnTS = true;
        }
    }
}
