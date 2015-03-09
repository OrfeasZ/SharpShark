using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets.Messages
{
    internal abstract class SharkMessage
    {
        public String Command { get; protected set; }

        public Dictionary<String, JToken> Blackbox { get; set; } 

        protected SharkMessage(String p_Command)
        {
            Command = p_Command;
        }
    }
}
