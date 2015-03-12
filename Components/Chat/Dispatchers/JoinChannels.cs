using System;
using System.Collections.Generic;
using GS.Lib.Network.Sockets.Messages;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private Dictionary<String, Dictionary<String, Object>> m_CurrentChannels;
        private bool m_ChannelListDirty;

        internal void JoinChannels(List<Object> p_Channels, Dictionary<String, JToken> p_Blackbox = null, bool p_DontUpdate = false, Action<SharkResponseMessage> p_Callback = null)
        {
            if (m_CurrentChannels == null)
            {
                m_CurrentChannels = new Dictionary<string, Dictionary<string, object>>();
                m_ChannelListDirty = false;
            }
            
            foreach (var s_Object in p_Channels)
            {
                var s_Channel = new Dictionary<String, Object>();

                if (s_Object is String)
                {
                    s_Channel.Add("sub", s_Object as String);
                    s_Channel.Add("overwrite_params", false);
                }
                else if (s_Object is Dictionary<String, Object>)
                {
                    var s_ObjectDict = s_Object as Dictionary<String, Object>;

                    if (s_ObjectDict.ContainsKey("sub"))
                        s_Channel.Add("sub", s_ObjectDict["sub"]);

                    Object s_CreateWhenDone;
                    Object s_TryToOwn;

                    if (s_ObjectDict.TryGetValue("create_when_dne", out s_CreateWhenDone) &&
                        s_ObjectDict.TryGetValue("try_to_own", out s_TryToOwn))
                    {
                        if (!((bool) s_CreateWhenDone) || !((bool) s_TryToOwn))
                        {
                            s_Channel.Add("create_when_dne", false);
                            s_Channel.Add("try_to_own", false);
                        }
                    }

                    s_Channel.Add("overwrite_params", false);
                }

                if (s_Channel.ContainsKey("sub"))
                {
                    if (!m_CurrentChannels.ContainsKey(s_Channel["sub"] as String))
                    {
                        m_CurrentChannels.Add(s_Channel["sub"] as String, s_Channel);
                        m_ChannelListDirty = true;
                    }
                    else
                    {
                        m_CurrentChannels[s_Channel["sub"] as String] = s_Channel;
                        m_ChannelListDirty = true;
                    }
                }
            }
            
            if (!p_DontUpdate)
                UpdateChannelList(p_Blackbox, p_Callback);
        }
    }
}
