using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void InitiateBroadcastDarkLaunch()
        {
            m_SocketClient.SendMessage(new BroadcastDarkLaunchRequest());
        }
    }
}
