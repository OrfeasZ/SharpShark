using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        internal class RequestBlackbox
        {
            public String Source { get; set; }

            public int PartsTotal { get; set; }

            public int PartID { get; set; }

            public bool Initial { get; set; }

            public bool Retried { get; set; }
        }

        public RequestParameters Params { get; set; }

        public RequestBlackbox Blackbox { get; set; }

        public SubscribeToMetaUsersRequest(List<String> p_UserIDs, int p_Parts, int p_PartID, bool p_Initial, bool p_Retried) 
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "userids",
                UserIDs = p_UserIDs
            };

            Blackbox = new RequestBlackbox()
            {
                Source = "subscribeToMetaUsers",
                PartsTotal = p_Parts,
                PartID = p_PartID,
                Initial = p_Initial,
                Retried = p_Retried
            };
        }
    }
}
