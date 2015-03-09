using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Events
{
    public class PubResultEvent : SharkEvent
    {
        public bool Success { get; set; }

        public Object Result { get; set; }

        public String Sub { get; set; }

        public Dictionary<String, JToken> Blackbox { get; set; }

        public PubResultEvent()
        {
            Blackbox = new Dictionary<string, JToken>();
        }
    }
}
