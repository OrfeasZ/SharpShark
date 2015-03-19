using System;

namespace GS.Lib.Models
{
    public class PlaylistData : SharkObject
    {
        public String UUID { get; set; }

        public String TSAdded { get; set; }

        public String About { get; set; }

        public String Picture { get; set; }

        public String TSModified { get; set; }

        public String Name { get; set; }

        public Int64 PlaylistID { get; set; }

        public Int64 SongCount { get; set; }

        public Int64 UserID { get; set; }
    }
}
