using System;

namespace GS.Lib.Events
{
    public class AuthenticationFailureEvent : SharkEvent
    {
        public String Error { get; set; }
    }
}
