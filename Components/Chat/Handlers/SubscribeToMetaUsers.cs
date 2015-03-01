using System;
using System.Collections.Generic;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private List<Int64> m_PendingOnlineUsers; 

        private void HandleSubscribeToMetaUsers(SharkResponseMessage p_Message)
        {
            var s_Message = p_Message.As<SubscribeToMetaUsersResponse>();

            var s_OnlineUsers = new List<Int64>();

            foreach (var s_User in s_Message.Success)
            {
                if (s_User.Status == "logged_in")
                    s_OnlineUsers.Add(Int64.Parse(s_User.UserID));
            }

            if (m_PendingOnlineUsers == null)
                m_PendingOnlineUsers = new List<long>();
            
            m_PendingOnlineUsers.AddRange(s_OnlineUsers);

            if (s_Message.Blackbox.PartID >= s_Message.Blackbox.PartsTotal)
            {
                //FetchUsersStatus(m_PendingOnlineUsers, true, s_Message.Blackbox.Initial);
                m_PendingOnlineUsers.Clear();
            }

            if (s_OnlineUsers.Count > 0)
                FetchUsersStatuses(m_PendingOnlineUsers, true, s_Message.Blackbox.Initial);
        }
    }
}
