using System;
using System.Collections.Generic;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    class BroadcastDarkLaunchRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Sub { get; set; }
            public List<String> Keys { get; set; }

            public RequestParameters()
            {
                Keys = new List<string>();
            }
        }

        public RequestParameters Params { get; set; }

        public Dictionary<String, String> Blackbox { get; set; } 

        public BroadcastDarkLaunchRequest() 
            : base("get")
        {
            Params = new RequestParameters()
            {
                Sub = "broadcastDarkLaunch",
                Keys = new List<String>() {"eligPer", "broadPer", "broadFreq", "largeChPer", "maxRandCh"}
            };

            Blackbox = new Dictionary<string, string>()
            {
                {"source", "broadcastDarkLaunch"}
            };
        }
    }
}
