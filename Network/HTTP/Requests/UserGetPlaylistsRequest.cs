using System;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class UserGetPlaylistsRequest : SharkObject
    {
        public Int64 UserID { get; set; }
    }
}
