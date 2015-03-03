using System;
using System.Collections.Generic;

namespace GS.Lib.Events
{
    public class SetResultEvent : SharkEvent
    {
        public bool Success { get; set; }

        public Object Result { get; set; }

        public Dictionary<String, Object> Blackbox { get; set; }

        public SetResultEvent()
        {
            Blackbox = new Dictionary<string, object>();
        }
    }
}
