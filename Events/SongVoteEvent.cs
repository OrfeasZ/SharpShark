using System;

namespace GS.Lib.Events
{
    public class SongVoteEvent : SharkEvent
    {
        public int CurrentVote { get; set; }
        public Int64 QueueSongID { get; set; }
    }
}
