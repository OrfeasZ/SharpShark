using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private Dictionary<Int64, List<String>> m_SubscribedMetaKeys; 

        private void FetchUsersStatuses(List<Int64> p_Users, bool p_SendSubscribe = true, bool p_Initial = false)
        {
            if (m_SubscribedMetaKeys == null)
                m_SubscribedMetaKeys = new Dictionary<long, List<string>>();

            if (!p_SendSubscribe)
            {
                var s_Keys = p_Users.Select(s_User => new UserSubscriptionData(s_User, new[] {"s"})).ToList();
                m_SocketClient.SendMessage(new FetchUsersStatusRequest(s_Keys));
                return;
            }

            foreach (var s_User in p_Users)
            {
                if (!m_SubscribedMetaKeys.ContainsKey(s_User))
                    m_SubscribedMetaKeys.Add(s_User, new List<string>(new[] { "s" }));
                
                if (!m_SubscribedMetaKeys[s_User].Contains("s"))
                    m_SubscribedMetaKeys[s_User].Add("s");
            }

            SubscribeToMetaUsersKeys(false, p_Initial);
        }
    }
}
