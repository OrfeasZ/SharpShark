using System;

namespace GS.Lib.Events
{
    internal class ChatErrorEvent : SharkEvent
    {
        public String Error { get; set; }

        public String Source { get; set; }

        public String Sub { get; set; }

        public String Command { get; set; }
    }
}
