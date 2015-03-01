using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SubscriptionResponse : SharkMessage
    {
        public Dictionary<String, JToken> Blackbox { get; set; }

        public List<Dictionary<String, Object>> Success { get; set; } 

        public SubscriptionResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
