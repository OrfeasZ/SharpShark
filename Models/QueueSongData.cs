using System;

namespace GS.Lib.Models
{
    public class QueueSongData : SharkObject
    {
        public Int64 QueueID { get; set; }

        public int Index { get; set; }

        public Int64 SongID { get; set; }

        public String SongName { get; set; }

        public Int64 ArtistID { get; set; }

        public String ArtistName { get; set; }

        public Int64 AlbumID { get; set; }

        public String AlbumName { get; set; }

        public int Votes { get; set; }
    }
}
