using System;
using System.Collections.Generic;
using GS.Lib.Models;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class SetSubscriptionParamsRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Sub { get; set; }
            public List<KeyValData> Keyvals { get; set; }
            public bool Silent { get; set; }
        }

        public Dictionary<String, Object> Params { get; set; }

        public SetSubscriptionParamsRequest(String p_Subscription, List<KeyValData> p_KeyVals, Dictionary<String, JToken> p_Blackbox = null, bool p_Silent = false)
            : base("set")
        {
            Params = new Dictionary<string, object>()
            {
                { "sub", p_Subscription },
                { "keyvals", p_KeyVals }
            };

            if (p_Silent)
                Params.Add("silent", true);

            Blackbox = p_Blackbox ?? new Dictionary<string, JToken>();
        }
    }
}
