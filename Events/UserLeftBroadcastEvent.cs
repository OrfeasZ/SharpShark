using System;
using GS.Lib.Models;

namespace GS.Lib.Events
{
    public class UserLeftBroadcastEvent : SharkEvent
    {
        public Int64 UserID { get; set; }

        public ChatUserData UserData { get; set; }
    }
}
