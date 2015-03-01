using System;
using System.Collections.Generic;
using GS.Lib.Models;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class SubscribeToMetaUsersKeysRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Type { get; set; }

            [JsonProperty("userid_keys")]
            public List<UserSubscription> KeyData { get; set; }
        }

        internal class RequestBlackbox
        {
            public String Type { get; set; }

            public String Source { get; set; }

            public bool Initial { get; set; }

            public bool Retried { get; set; }
        }

        public RequestParameters Params { get; set; }

        public RequestBlackbox Blackbox { get; set; }

        public SubscribeToMetaUsersKeysRequest(List<UserSubscription> p_KeyData, bool p_Initial, bool p_Retried)
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "userid_keys",
                KeyData = p_KeyData
            };

            Blackbox = new RequestBlackbox()
            {
                Source = "subscribeToMetaUsersKeys",
                Type = "Users",
                Initial = p_Initial,
                Retried = p_Retried
            };
        }
    }
}
