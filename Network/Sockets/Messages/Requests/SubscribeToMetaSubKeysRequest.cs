using System;
using System.Collections.Generic;
using GS.Lib.Models;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    class SubscribeToMetaSubKeysRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Type { get; set; }

            [JsonProperty("sub_keys")]
            public List<SubscriptionKeys> SubKeys { get; set; } 
        }

        public RequestParameters Params { get; set; }

        public SubscribeToMetaSubKeysRequest(List<SubscriptionKeys> p_Keys) 
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "sub_keys",
                SubKeys = p_Keys
            };
        }
    }
}
