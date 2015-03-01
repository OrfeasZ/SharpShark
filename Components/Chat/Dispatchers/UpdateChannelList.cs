using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void UpdateChannelList(Dictionary<String, Object> p_Blackbox = null)
        {
            if (!m_ChannelListDirty)
                return;

            m_SocketClient.SendMessage(GetChannelSubData(p_Blackbox));
            m_ChannelListDirty = false;
        }

        private ChannelSubRequest GetChannelSubData(Dictionary<String, Object> p_Blackbox = null)
        {
            var s_Channels = m_CurrentChannels.Select(p_Pair => p_Pair.Value).ToList();

            Object s_Source;
            if (p_Blackbox != null && p_Blackbox.TryGetValue("source", out s_Source) && s_Source is String &&
                (String) s_Source == "reconnect" && s_Channels.Count == 0)
                return null;
            
            return new ChannelSubRequest(s_Channels, p_Blackbox);
        }
    }
}
