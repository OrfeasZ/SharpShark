using System;
using System.Collections.Generic;
using GS.Lib.Network.Sockets.Messages.Requests;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        internal void GetSubscriberList(String p_SubList, bool p_WithExtra = true, Dictionary<String, JToken> p_Blackbox = null)
        {
            if (p_Blackbox == null)
            {
                p_Blackbox = new Dictionary<string, JToken>()
                {
                    {"sub", p_SubList}
                };
            }
            else
            {
                if (p_Blackbox.ContainsKey("sub"))
                    p_Blackbox["sub"] = p_SubList;
                else
                    p_Blackbox.Add("sub", p_SubList);
            }

            m_SocketClient.SendMessage(new GetSubscriberListRequest(p_SubList, p_WithExtra, p_Blackbox));
        }
    }
}
