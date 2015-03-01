using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class RestoreLookupRequest : SharkMessage
    {
        internal class RequestParams
        {
            public List<String> Keys { get; set; }

            [JsonProperty("userid")]
            public String UserID { get; set; }

            public RequestParams()
            {
                Keys = new List<string>();
            }
        }

        public Dictionary<String, String> Blackbox { get; set; }

        public RequestParams Params { get; set; }

        public RestoreLookupRequest(Int64 p_UserID) 
            : base("get")
        {
            Blackbox = new Dictionary<string, string>()
            {
                { "source", "restore" }
            };

            Params = new RequestParams
            {
                UserID = p_UserID.ToString()
            };
            Params.Keys.Add("s");
        }
    }
}
