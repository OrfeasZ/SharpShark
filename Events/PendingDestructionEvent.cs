using System;

namespace GS.Lib.Events
{
    public class PendingDestructionEvent : SharkEvent
    {
        public Int64 TimeLeft { get; set; }
    }
}
