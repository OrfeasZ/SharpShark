using System;

namespace GS.Lib.Models
{
    public class QueueSongData : SharkObject
    {
        public Int64 QueueID { get; set; }

        public int Index { get; set; }

        public Int64 SongID { get; set; }

        public int Votes { get; set; }
    }
}
