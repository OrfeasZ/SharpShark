using System;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class ErrorResponse<T> : BasicResponse
    {
        public String Error { get; set; }

        public T Blackbox { get; set; }

        public ErrorResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
