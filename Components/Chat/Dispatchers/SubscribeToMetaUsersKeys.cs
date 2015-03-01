using System;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void SubscribeToMetaUsersKeys(bool p_Retry = false, bool p_Initial = false)
        {
            var s_Keys = m_SubscribedMetaKeys.Select(p_User => new UserSubscriptionData(p_User.Key, p_User.Value)).ToList();

            if (s_Keys.Count == 0)
                return;

            if (p_Retry)
                throw new NotImplementedException("We don't currently support request retrying.");

            m_SocketClient.SendMessage(new SubscribeToMetaUsersKeysRequest(s_Keys, p_Initial, false));
        }
    }
}
