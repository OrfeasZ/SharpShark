using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GS.Lib.Models;
using GS.Lib.Network.Sockets;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent : EventsComponent
    {
        internal Dictionary<String, int> ChatServers { get; set; }

        internal MasterStatus LoggedInMaster { get; set; }

        internal String LastMasterUUID { get; set; }

        public String UID { get; set; }

        private readonly Client m_SocketClient;

        private readonly Dictionary<String, Action<SharkResponseMessage>> m_Handlers;

        private const UInt16 c_ServerPort = 443;
        private const String c_ChatServerChannelPublicPrefix = "bcast:p:";
        private const String c_ChatServerChannelPrefix = "bcast:";

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
            m_Handlers.Add("set", HandleSet);
            m_Handlers.Add("meta_sub", HandleMetaSub);
            m_Handlers.Add("sub", HandleSub);
            m_Handlers.Add("get", HandleGet);
            m_Handlers.Add("pub", HandlePub);
            m_Handlers.Add("push", HandlePush);
        }

        public bool Connect()
        {
            if (Library.User.Data == null)
                return false;

            if (ChatServers.Count == 0)
                return false;

            LoggedInMaster = null;

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
                false, Library.User.Data.UserID));
        }

        private void OnDisconnected(object p_Sender, EventArgs p_EventArgs)
        {
            UID = null;
            // TODO: Reconnection
        }

        private void OnMessageProcessed(object p_Sender, SharkResponseMessage p_Message)
        {
            Action<SharkResponseMessage> s_Handler;
            
            if (!m_Handlers.TryGetValue(p_Message.Command, out s_Handler))
            {
                Debug.WriteLine(String.Format("Received an unhandled command: {0}", p_Message.Command));
                return;
            }

            //Debug.WriteLine(String.Format("Received a known command: {0}", p_Message.Command));

            s_Handler(p_Message);
        }

        private String GetSessionPart()
        {
            if (String.IsNullOrWhiteSpace(Library.User.SessionID))
                return "";

            using (var s_MD5 = MD5.Create())
            {
                var s_HashedPassword = s_MD5.ComputeHash(Encoding.ASCII.GetBytes(Library.User.SessionID));
                return BitConverter.ToString(s_HashedPassword).Replace("-", "").ToLower().Substring(0, 6);
            }
        }

        internal String GetChatChannel(String p_ChannelID, bool p_Public = false)
        {
            if (p_ChannelID == null)
                p_ChannelID = "";

            if (p_ChannelID.Contains(":"))
                p_ChannelID = p_ChannelID.Substring(0, p_ChannelID.IndexOf(":"));

            return (p_Public ? c_ChatServerChannelPublicPrefix : c_ChatServerChannelPrefix) + p_ChannelID;
        }
    }
}
