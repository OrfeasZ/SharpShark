using System;
using System.Collections.Generic;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class BlackboxResponse : SharkMessage
    {
        public BlackboxResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
