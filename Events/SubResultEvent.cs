using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Events
{
    public class SubResultEvent : SharkEvent
    {
        public Dictionary<String, Object> ChannelParameters { get; set; }

        public Dictionary<String, String> ChannelErrors { get; set; }

        public Dictionary<String, Object> LatestMessages { get; set; }

        public Dictionary<String, Dictionary<String, Object>> Channels { get; set; }

        public Dictionary<String, JToken> Blackbox { get; set; }

        public SubResultEvent()
        {
            ChannelParameters = new Dictionary<string, object>();
            ChannelErrors = new Dictionary<string, string>();
            LatestMessages = new Dictionary<string, object>();
            Channels = new Dictionary<string, Dictionary<string, object>>();
            Blackbox = new Dictionary<string, JToken>();
        }
    }
}
