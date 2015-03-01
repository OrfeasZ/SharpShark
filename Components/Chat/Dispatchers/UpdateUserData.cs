using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void UpdateUserData(ChatUserData p_Data = null)
        {
            if (p_Data == null && Library.User.Data != null)
                p_Data = Library.User.Data.ChatUserData;

            m_SocketClient.SendMessage(new UpdateUserDataRequest(p_Data));
        }
    }
}
