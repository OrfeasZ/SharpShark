using System;
using System.Diagnostics;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Generic;
using GS.Lib.Network.Sockets.Messages.Responses;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandleMetaSub(SharkResponseMessage p_Message)
        {
            switch (p_Message.As<BasicResponse>().Type)
            {
                case "error":
                    {
                        Debug.WriteLine(String.Format("Set failed. Error: {0}", p_Message.As<ErrorResponse<Object>>().Error));

                        // TODO: Retry request if we have enough data.

                        break;
                    }

                case "success":
                    {
                        var s_Message = p_Message.As<SuccessResponse<JToken, PartitionedBlackbox>>();

                        if (s_Message.Blackbox != null)
                        {
                            if (s_Message.Blackbox.Source == "subscribeToMetaUsers")
                            {
                                HandleSubscribeToMetaUsers(p_Message);
                                break;
                            }
                        }

                        Debug.WriteLine("Unknown metasub response type received!");

                        break;
                    }

                default:
                    {
                        Debug.WriteLine("Unknown metasub response type received!");
                        break;
                    }
            }
        }
    }
}
