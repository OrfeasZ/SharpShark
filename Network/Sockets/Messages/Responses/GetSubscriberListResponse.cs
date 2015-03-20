using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class GetSubscriberListResponse : SharkMessage
    {
        public String Type { get; set; }

        [JsonProperty("sub_list_with_extra")]
        public JArray SubList { get; set; }

        public GetSubscriberListResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
