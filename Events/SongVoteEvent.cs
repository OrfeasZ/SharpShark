using System;

namespace GS.Lib.Events
{
    public class SongVoteEvent : SharkEvent
    {
        public int VoteChange { get; set; }
        public int CurrentVotes { get; set; }
        public Int64 QueueSongID { get; set; }
    }
}
