using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Events
{
    internal class SetResultEvent : SharkEvent
    {
        public bool Success { get; set; }

        public Object Result { get; set; }

        public Dictionary<String, JToken> Blackbox { get; set; }

        public SetResultEvent()
        {
            Blackbox = new Dictionary<string, JToken>();
        }
    }
}
