using System;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetPlaylistByIDRequest : SharkObject
    {
        public String PlaylistID { get; set; }

        public GetPlaylistByIDRequest(Int64 p_ID)
        {
            PlaylistID = p_ID.ToString();
        }
    }
}
