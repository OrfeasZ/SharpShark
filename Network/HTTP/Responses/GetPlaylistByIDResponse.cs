using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Responses
{
    public class GetPlaylistByIDResponse : SharkObject
    {
        public String UUID { get; set; }

        public String TSAdded { get; set; }

        public String About { get; set; }

        public Int64 SubscriberCount { get; set; }

        public String Picture { get; set; }

        public Int64 LastModifiedBy { get; set; }

        public Int64 TSModified { get; set; }

        public String Name { get; set; }

        public Int64 PlaylistID { get; set; }

        public Int64 SongCount { get; set; }

        public Int64 UserID { get; set; }

        public List<String> AlbumFiles { get; set; }

        public List<StatusSongData> Songs { get; set; } 
    }
}
