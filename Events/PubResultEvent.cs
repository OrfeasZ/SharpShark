using System;
using System.Collections.Generic;

namespace GS.Lib.Events
{
    public class PubResultEvent : SharkEvent
    {
        public bool Success { get; set; }

        public Object Result { get; set; }

        public String Sub { get; set; }

        public Dictionary<String, Object> Blackbox { get; set; }

        public PubResultEvent()
        {
            Blackbox = new Dictionary<string, object>();
        }
    }
}
