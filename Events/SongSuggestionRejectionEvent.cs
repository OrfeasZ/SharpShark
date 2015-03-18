using System;
using GS.Lib.Models;

namespace GS.Lib.Events
{
    public class SongSuggestionRejectionEvent : SharkEvent
    {
        public Int64 SongID { get; set; }

        public ChatUserData User { get; set; }

        public Int64 UserID { get; set; }
    }
}
