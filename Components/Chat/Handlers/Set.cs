using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandleSet(SharkResponseMessage p_Message)
        {
            switch (p_Message.As<BasicResponse>().Type)
            {
                case "error":
                    {
                        Debug.WriteLine(String.Format("Set failed. Error: {0}", p_Message.As<ErrorResponse<Object>>().Error));
                        break;
                    }

                case "success":
                    {
                        var s_Message = p_Message.As<SuccessResponse<String, Object>>();
                        break;
                    }

                default:
                    {
                        Debug.WriteLine("Unknown set response type received!");
                        break;
                    }
            }
        }
    }
}
