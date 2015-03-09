using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        internal void SetSubscriptionParameters(String p_Subscription, Dictionary<String, Object> p_Params, Dictionary<String, JToken> p_Blackbox = null, bool p_Silent = false)
        {
            if (p_Blackbox == null)
            {
                p_Blackbox = new Dictionary<string, JToken>()
                {
                    { "source", "setSubParams" },
                    { "sub", p_Subscription }
                };
            }

            var s_KeyVals = p_Params.Select(p_Pair => new KeyValData() { Key = p_Pair.Key, Value = p_Pair.Value }).ToList();

            m_SocketClient.SendMessage(new SetSubscriptionParamsRequest(p_Subscription, s_KeyVals, p_Blackbox, p_Silent));
        }
    }
}
