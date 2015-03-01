using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class FetchUsersStatusRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public List<UserSubscriptionData> Multikeys { get; set; } 
        }

        public RequestParameters Params { get; set; }

        public Dictionary<String, String> Blackbox { get; set; } 

        public FetchUsersStatusRequest(List<UserSubscriptionData> p_KeyData) 
            : base("multiget")
        {
            Params = new RequestParameters()
            {
                Multikeys = p_KeyData
            };

            Blackbox = new Dictionary<string, string>()
            {
                { "source", "fetchUsersStatuses" }
            };
        }
    }
}
