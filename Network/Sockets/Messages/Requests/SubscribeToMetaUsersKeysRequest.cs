using System;
using System.Collections.Generic;
using GS.Lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public RequestParameters Params { get; set; }

        public SubscribeToMetaUsersKeysRequest(List<UserSubscription> p_KeyData, bool p_Initial, bool p_Retried)
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "userid_keys",
                KeyData = p_KeyData
            };

            Blackbox = new Dictionary<string, JToken>()
            {
                { "source", "subscribeToMetaUsersKeys" },
                { "type", "Users" },
                { "initial", p_Initial },
                { "retried", p_Retried }
            };
        }
    }
}
