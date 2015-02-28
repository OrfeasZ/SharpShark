using System;
using System.Diagnostics;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;

namespace GS.Lib.Components
{
    internal partial class ChatComponent
    {
        private void HandleIdentify(SharkResponseMessage p_Message)
        {
            switch (p_Message.As<BasicResponse>().Type)
            {
                case "error":
                {
                    Debug.WriteLine(String.Format("Failed to identify with the Chat Service. Error: {0}", p_Message.As<ErrorResponse>().Error));
                    break;
                }

                case "success":
                {
                    Debug.WriteLine("Successfully authenticated with the Chat Service.");
                    var s_Message = p_Message.As<IdentifyResponse>();

                    UID = s_Message.Success.ID.UID;

                    break;
                }

                default:
                {
                    Debug.WriteLine("Unknown response type received!");
                    break;
                }
            }
        }
    }
}
