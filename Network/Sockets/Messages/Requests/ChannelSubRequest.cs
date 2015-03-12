using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class ChannelSubRequest : SharkMessage
    {
        internal class RequestParams
        {
            public bool Add { get; set; }

            public List<Dictionary<String, Object>> Subs { get; set; }

            public RequestParams()
            {
                Subs = new List<Dictionary<string, object>>();
            }
        }

        public RequestParams Params { get; set; }

        public ChannelSubRequest(List<Dictionary<String, Object>> p_Channels, Dictionary<String, JToken> p_Blackbox = null) 
            : base("sub")
        {
            Params = new RequestParams()
            {
                Subs = p_Channels,
                Add = true
            };

            Blackbox = p_Blackbox ?? new Dictionary<string, JToken>();
        }
    }
}
