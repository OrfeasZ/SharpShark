using System;
using System.Collections.Generic;
using GS.Lib.Models;

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

        public RequestParameters Params { get; set; }

        public Dictionary<String, Object> Blackbox { get; set; } 

        public SetSubscriptionParamsRequest(String p_Subscription, List<KeyValData> p_KeyVals, Dictionary<String, Object> p_Blackbox = null, bool p_Silent = false)
            : base("set")
        {
            Params = new RequestParameters()
            {
                Sub = p_Subscription,
                Keyvals = p_KeyVals,
                Silent = p_Silent
            };

            Blackbox = p_Blackbox ?? new Dictionary<string, object>();
        }
    }
}
