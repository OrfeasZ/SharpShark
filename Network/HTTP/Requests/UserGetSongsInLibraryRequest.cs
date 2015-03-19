using System;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class UserGetSongsInLibraryRequest : SharkObject
    {
        public Int64 Page { get; set; }

        public Int64 UserID { get; set; }
    }
}
