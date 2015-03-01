using System;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetFavoritesRequest : SharkObject
    {
        public String OfWhat { get; set; }

        public Int64 UserID { get; set; }
    }
}
