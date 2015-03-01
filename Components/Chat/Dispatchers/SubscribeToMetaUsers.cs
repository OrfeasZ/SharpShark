using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private List<String> m_SubscribedMetaUserIDs;

        private const int c_MaxIDsPerCall = 1300;

        private void SubscribeToMetaUsers(List<String> p_UserIDs, bool p_Replace, bool p_InitialSubscribe)
        {
            if (m_SubscribedMetaUserIDs == null || p_Replace)
            {
                m_SubscribedMetaUserIDs = p_UserIDs;
            }
            else
            {
                foreach (var s_UserID in p_UserIDs)
                    if (!m_SubscribedMetaUserIDs.Contains(s_UserID))
                        m_SubscribedMetaUserIDs.Add(s_UserID);
            }

            if (m_SubscribedMetaUserIDs.Count == 0)
                return;

            foreach (var s_Request in FormSubscribeToMetaUsersRequest(false, p_InitialSubscribe))
                m_SocketClient.SendMessage(s_Request);
        }

        private IEnumerable<SubscribeToMetaUsersRequest> FormSubscribeToMetaUsersRequest(bool p_Retried, bool p_InitialSubscribe)
        {
            var s_Parts = m_SubscribedMetaUserIDs
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / c_MaxIDsPerCall)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            for (var i = 0; i < s_Parts.Count; ++i)
            {
                var s_Request = new SubscribeToMetaUsersRequest(s_Parts[i], s_Parts.Count, i + 1, p_InitialSubscribe, p_Retried);
                yield return s_Request;
            }
        }
    }
}
