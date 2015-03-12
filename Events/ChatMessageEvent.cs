using System;

namespace GS.Lib.Events
{
    public class ChatMessageEvent : SharkEvent
    {
        public String DestinationChannel { get; set; }
        public Int64 UserID { get; set; }
        public String UserName { get; set; }
        public String ChatMessage { get; set; }
    }
}
