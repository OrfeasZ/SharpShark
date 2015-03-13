using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Events
{
    internal class SubUpdateEvent : SharkEvent
    {
        public String Type { get; set; }

        public String Sub { get; set; }

        public Dictionary<String, JToken> Params { get; set; }

        public Dictionary<String, JToken> ID { get; set; }

        public Dictionary<String, JToken> Value { get; set; }

        public String NewName { get; set; }
    }
}
