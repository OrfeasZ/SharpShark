using System;
using System.Collections.Generic;

namespace GS.Lib.Events
{
    public class SubUpdateEvent : SharkEvent
    {
        public String Type { get; set; }

        public String Sub { get; set; }

        public Dictionary<String, Object> Params { get; set; }

        public Dictionary<String, Object> ID { get; set; }

        public Dictionary<String, Object> Value { get; set; }

        public String NewName { get; set; }
    }
}
