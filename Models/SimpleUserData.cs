using System;

namespace GS.Lib.Models
{
    public class SimpleUserData : SharkObject
    {
        public Int64 UserID { get; set; }

        public ChatUserData UserData { get; set; }
    }
}
