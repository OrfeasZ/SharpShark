using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class SubscribeToMetaUsersRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Type { get; set; }

            [JsonProperty("userids")]
            public List<String> UserIDs { get; set; } 
        }


        public RequestParameters Params { get; set; }

        public SubscribeToMetaUsersRequest(List<String> p_UserIDs, int p_Parts, int p_PartID, bool p_Initial, bool p_Retried) 
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "userids",
                UserIDs = p_UserIDs
            };

            Blackbox = new Dictionary<string, JToken>()
            {
                { "source", "subscribeToMetaUsers" },
                { "partsTotal", p_Parts },
                { "partID", p_PartID },
                { "initial", p_Initial },
                { "retried", p_Retried }
            };
        }
    }
}
