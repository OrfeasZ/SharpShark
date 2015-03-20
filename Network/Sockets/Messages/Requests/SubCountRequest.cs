using System;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class SubCountRequest : SharkMessage
    {
        internal class RequestParameters
        {
            internal class SubCount
            {
                public String Name { get; set; }

                public String Type { get; set; }
            }

            public String Type { get; set; }

            [JsonProperty("sub_count")]
            public SubCount Count { get; set; }
        }

        public RequestParameters Params { get; set; }

        public SubCountRequest(String p_Channel) 
            : base("info")
        {
            Params = new RequestParameters()
            {
                Type = "sub_count",
                Count = new RequestParameters.SubCount()
                {
                    Name = p_Channel,
                    Type = "sub"
                }
            };
        }
    }
}
