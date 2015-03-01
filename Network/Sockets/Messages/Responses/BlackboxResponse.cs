using System;
using System.Collections.Generic;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class BlackboxResponse : SharkMessage
    {
        public Dictionary<String, Object> Blackbox { get; set; }

        public BlackboxResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
