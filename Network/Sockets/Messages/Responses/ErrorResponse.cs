using System;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    class ErrorResponse : BasicResponse
    {
        public String Error { get; set; }

        public ErrorResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
