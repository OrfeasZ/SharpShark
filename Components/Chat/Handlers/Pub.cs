using System;
using System.Collections.Generic;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandlePub(SharkResponseMessage p_Message)
        {
            var s_BasicResponse = p_Message.As<BasicResponse>();
            var s_ReturnResponse = p_Message.As<ReturnResponse>();
            var s_ErrorResponse = p_Message.As<ErrorResponse>();

            if (s_BasicResponse.Type == "return" && s_ReturnResponse != null && s_ReturnResponse.Return != null &&
                s_ReturnResponse.Return.Type == JTokenType.Array && s_ReturnResponse.Return.ToObject<JArray>().Count > 0)
            {
                var s_ReturnValue = s_ReturnResponse.Return.ToObject<JArray>()[0].ToObject<Dictionary<String, Object>>();

                if (Library.User.Data != null && s_ReturnValue.ContainsKey("name") &&
                    ((Library.User.Data.ArtistID > 0 &&
                      s_ReturnValue["name"] as String == "artist:" + Library.User.Data.ArtistID) ||
                     Library.User.Data.UserID > 0 &&
                     s_ReturnValue["name"] as String == "user:" + Library.User.Data.UserID))
                    return;

                if (s_ReturnValue.ContainsKey("error"))
                {
                    DispatchEvent((int) ChatEvent.ChatError, new ChatErrorEvent
                    {
                        Error = s_ReturnValue["error"] as String,
                        Source =
                            s_ReturnResponse.Blackbox != null
                                ? (s_ReturnResponse.Blackbox.ContainsKey("source")
                                    ? s_ReturnResponse.Blackbox["source"].Value<String>()
                                    : "")
                                : "",
                        Sub = s_ReturnValue.ContainsKey("name") ? s_ReturnValue["name"] as String : "",
                        Command = "Pub"
                    });
                }

                DispatchEvent((int) ChatEvent.PubResult, new PubResultEvent()
                {
                    Success = !s_ReturnValue.ContainsKey("error"),
                    Result = s_ReturnValue,
                    Sub = s_ReturnValue.ContainsKey("name") ? s_ReturnValue["name"] as String : "",
                    Blackbox = s_ReturnResponse.Blackbox
                });
            }
            else if (s_BasicResponse.Type == "error" && s_ErrorResponse != null)
            {
                DispatchEvent((int)ChatEvent.ChatError, new ChatErrorEvent
                {
                    Error = s_ErrorResponse.Error,
                    Source =
                        s_ReturnResponse.Blackbox != null
                            ? (s_ReturnResponse.Blackbox.ContainsKey("source")
                                ? s_ReturnResponse.Blackbox["source"].Value<String>()
                                : "")
                            : "",
                    Command = "Pub"
                });

                DispatchEvent((int)ChatEvent.PubResult, new PubResultEvent()
                {
                    Success = false,
                    Result = p_Message,
                    Blackbox = s_ReturnResponse.Blackbox
                });
            }
        }
    }
}
