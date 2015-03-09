using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class ReturnResponse : SharkMessage
    {
        public String Type { get; set; }

        public JToken Return { get; set; }

        public ReturnResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
