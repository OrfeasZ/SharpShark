using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class GetSubscriptionDataRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Sub { get; set; }

            public List<String> Keys { get; set; } 
        }

        public RequestParameters Params { get; set; }

        public GetSubscriptionDataRequest(String p_Subscription, IEnumerable<String> p_Keys) 
            : base("get")
        {
            Params = new RequestParameters()
            {
                Sub = p_Subscription,
                Keys = p_Keys.ToList()
            };
        }
    }
}
