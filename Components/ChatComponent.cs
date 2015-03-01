using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GS.Lib.Network.Sockets;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent : SharkComponent
    {
        internal Dictionary<String, int> ChatServers { get; set; }

        public String UID { get; set; }

        private readonly Client m_SocketClient;

        private const UInt16 c_ServerPort = 443;

        private readonly Dictionary<String, Action<SharkResponseMessage>> m_Handlers;

        internal ChatComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            ChatServers = new Dictionary<string, int>();

            m_SocketClient = new Client();

            m_SocketClient.OnConnected += OnConnected;
            m_SocketClient.OnDisconnected += OnDisconnected;
            m_SocketClient.OnMessageProcessed += OnMessageProcessed;

            m_Handlers = new Dictionary<string, Action<SharkResponseMessage>>();

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            m_Handlers.Add("identify", HandleIdentify);
        }

        public bool Connect()
        {
            if (Library.User.Data == null)
                return false;

            if (ChatServers.Count == 0)
                return false;

            var s_Server = DetermineBestServer();

            return m_SocketClient.Connect(s_Server, c_ServerPort);
        }

        private String DetermineBestServer()
        {
            var s_Server = ChatServers.First().Key;
            var s_ServerValue = ChatServers.First().Value;

            foreach (var s_Pair in ChatServers)
            {
                if (s_Pair.Value <= s_ServerValue)
                    continue;

                s_ServerValue = s_Pair.Value;
                s_Server = s_Pair.Key;
            }

            return s_Server;
        }

        private void OnConnected(object p_Sender, bool p_Success)
        {
            if (!p_Success)
            {
                Console.WriteLine("Connection to the chat service failed.");
            }

            // TODO: Additional checks because when reconnecting we need to do thing differently.
            m_SocketClient.SendMessage(new IdentifyRequest(Library.User.Data.ChatUserData,
                Library.User.Data.ChatUserDataSig, Library.User.SessionID,
                Library.User.UUID, false, Library.User.Data.UserID));
        }

        private void OnDisconnected(object p_Sender, EventArgs p_EventArgs)
        {
            // TODO: Reconnection
        }

        private void OnMessageProcessed(object p_Sender, SharkResponseMessage p_Message)
        {
            Action<SharkResponseMessage> s_Handler;

            if (!m_Handlers.TryGetValue(p_Message.Command, out s_Handler))
            {
                Debug.WriteLine("Received an unhandled command: {0}", p_Message.Command);
                return;
            }

            s_Handler(p_Message);
        }
    }
}
