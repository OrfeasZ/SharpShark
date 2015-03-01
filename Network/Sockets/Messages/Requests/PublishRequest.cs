using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class PublishRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Type { get; set; }
            public Object Value { get; set; }
            public List<Subscription> Subs { get; set; }
            public bool Async { get; set; }
            public bool Persist { get; set; }
        }

        public RequestParameters Params { get; set; }

        public Dictionary<String, Object> Blackbox { get; set; } 

        public PublishRequest(List<Subscription> p_Subs, Object p_Value, bool p_Async, bool p_Persist, String p_Source = null, Object p_ExtraData = null) 
            : base("pub")
        {
            Params = new RequestParameters()
            {
                Type = "data",
                Value = p_Value,
                Subs = p_Subs,
                Async = p_Async,
                Persist = p_Persist
            };

            Blackbox = new Dictionary<string, object>();

            if (!String.IsNullOrWhiteSpace(p_Source))
                Blackbox.Add("source", p_Source);

            if (p_ExtraData != null)
                Blackbox.Add("extraData", p_ExtraData);
        }
    }
}
