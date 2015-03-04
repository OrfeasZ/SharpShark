using System;
using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void UpdateUserData(Dictionary<String, Object> p_Data = null)
        {
            if (p_Data == null && Library.User.Data != null)
            {
                p_Data = new Dictionary<string, object>()
                {
                    { "p", Library.User.Data.ChatUserData.Picture },
                    { "n", Library.User.Data.ChatUserData.Username },
                    { "y", Library.User.Data.ChatUserData.Unknown != "0" }
                };
            }

            m_SocketClient.SendMessage(new UpdateUserDataRequest(p_Data));
        }
    }
}
