using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using GS.Lib.Enums;
using GS.Lib.Models;
using GS.Lib.Network.Sockets;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Requests;
using GS.Lib.Util;

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

        private bool m_ShouldReconnect;
        private bool m_Reconnect;

        private Timer m_ReconnectionTimer;

        internal ChatComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            m_ReconnectionTimer = new Timer();
            ChatServers = new Dictionary<string, int>();

            m_SocketClient = new Client();

            m_SocketClient.OnConnected += OnConnected;
            m_SocketClient.OnDisconnected += OnDisconnected;
            m_SocketClient.OnMessageProcessed += OnMessageProcessed;

            m_Handlers = new Dictionary<string, Action<SharkResponseMessage>>();
            m_ShouldReconnect = true;
            m_Reconnect = false;

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

        public bool Connect(bool p_Reconnect)
        {
            if (Library.User.Data == null)
                return false;

            if (ChatServers.Count == 0)
                return false;

            m_Reconnect = p_Reconnect;
            LoggedInMaster = null;

            var s_Server = DetermineBestServer();

            var s_Result = m_SocketClient.Connect(s_Server, c_ServerPort);

            if (!s_Result && m_Reconnect)
            {
                Reconnect();
                return true;
            }

            return s_Result;
        }

        private void Reconnect()
        {
            Trace.WriteLine("Reconnecting to manatee in 5 seconds.");

            m_ReconnectionTimer.Dispose();
            m_ReconnectionTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = false
            };

            m_ReconnectionTimer.Elapsed += OnReconnect;
            m_ReconnectionTimer.Start();
        }

        private void OnReconnect(object p_Sender, ElapsedEventArgs p_ElapsedEventArgs)
        {
            if (m_SocketClient.IsConnected)
                return;

            Trace.WriteLine("Reconnecting to manatee...");

            LoggedInMaster = null;

            if (!m_SocketClient.Connect(DetermineBestServer(), c_ServerPort) && m_Reconnect)
                Reconnect();
        }

        public void Disconnect()
        {
            m_ShouldReconnect = false;

            if (!m_SocketClient.IsConnected)
                return;

            m_SocketClient.Disconnect();
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
                return;
            }

            Library.DispatchEvent(ClientEvent.Connected, null);

            m_SocketClient.SendMessage(new IdentifyRequest(Library.User.Data.ChatUserData,
                Library.User.Data.ChatUserDataSig, Library.User.SessionID,
                false, Library.User.Data.UserID));
        }

        private void OnDisconnected(object p_Sender, EventArgs p_EventArgs)
        {
            UID = null;
            LoggedInMaster = null;
            Library.Broadcast.ActiveBroadcastID = null;
            Library.Broadcast.PlayingSongID = 0;
            Library.Broadcast.PlayingSongQueueID = 0;

            Library.DispatchEvent(ClientEvent.Disconnected, null);

            // TODO: Verify re-connection works.
            if (m_ShouldReconnect && m_Reconnect)
                Reconnect();
        }

        private void OnMessageProcessed(object p_Sender, SharkResponseMessage p_Message)
        {
            Action<SharkResponseMessage> s_Handler;
            
            if (!m_Handlers.TryGetValue(p_Message.Command, out s_Handler))
            {
                Trace.WriteLine(String.Format("Received an unhandled command: {0}", p_Message.Command));
                return;
            }

            //Trace.WriteLine(String.Format("Received a known command: {0}", p_Message.Command));

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

        public void SendChatMessage(string p_Message)
        {
            if (Library.User.Data == null || Library.User.Data.UserID <= 0 ||
                Library.Broadcast.ActiveBroadcastID == null)
                return;

            foreach (var s_Message in p_Message.Trim().SplitInParts(255))
            {
                PublishToChannels(new List<string>() { GetChatChannel(Library.Broadcast.ActiveBroadcastID, true) }, new Dictionary<String, Object>()
                {
                    { "type", "chat" },
                    { "data", s_Message },
                    { "ignoreTag", false },
                    { "playingSongID", Library.Broadcast.PlayingSongID }
                }, p_Persist: true);
            }
        }
    }
}
