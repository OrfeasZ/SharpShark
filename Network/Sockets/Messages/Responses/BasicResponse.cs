﻿using System;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class BasicResponse : SharkMessage
    {
        public String Type { get; set; }

        public BasicResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
