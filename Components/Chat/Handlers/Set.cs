using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandleSet(SharkResponseMessage p_Message)
        {
            var s_BasicResponse = p_Message.As<BasicResponse>();
            var s_SuccessResponse = p_Message.As<SuccessResponse<String>>();

            switch (s_BasicResponse.Type)
            {
                case "error":
                    {
                        Debug.WriteLine(String.Format("Set failed. Error: {0}", p_Message.As<ErrorResponse>().Error));
                        break;
                    }

                case "success":
                    {

                        break;
                    }

                default:
                    {
                        Debug.WriteLine("Unknown set response type received!");
                        break;
                    }
            }

            DispatchEvent((int) ChatEvent.SetResult, new SetResultEvent()
            {
                Success = s_BasicResponse.Type == "success" && s_SuccessResponse != null && s_SuccessResponse.Success != null,
                Result = p_Message,
                Blackbox = s_SuccessResponse != null ? s_SuccessResponse.Blackbox : null
            });
        }
    }
}
