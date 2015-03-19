using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Responses
{
    internal class UserGetSongsInLibraryResponse
    {
        public bool HasMore { get; set; }

        public Int64 MaxSongs { get; set; }

        public List<ResultData> Songs { get; set; } 

        public Int64 TSModified { get; set; }
    }
}
