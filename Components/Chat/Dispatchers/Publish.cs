﻿using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        internal void PublishToChannels<T>(List<String> p_Subscriptions, T p_Value, bool p_Async = false, bool p_Persist = false, String p_Source = null, Object p_ExtraData = null, Action<SharkResponseMessage> p_Callback = null)
        {
            var s_Subscriptions = p_Subscriptions.Select(p_Subscription => new Subscription(p_Subscription)).ToList();
            m_SocketClient.SendMessage(new PublishRequest(s_Subscriptions, p_Value, p_Async, p_Persist, p_Source, p_ExtraData), p_Callback);
        }
    }
}
