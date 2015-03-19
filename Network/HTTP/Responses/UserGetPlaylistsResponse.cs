using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Responses
{
    internal class UserGetPlaylistsResponse : SharkObject
    {
        public List<PlaylistData> Playlists { get; set; } 
    }
}
