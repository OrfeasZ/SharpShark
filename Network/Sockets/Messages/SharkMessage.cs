using System;

namespace GS.Lib.Network.Sockets.Messages
{
    internal abstract class SharkMessage
    {
        public String Command { get; protected set; }

        protected SharkMessage(String p_Command)
        {
            Command = p_Command;
        }
    }
}
