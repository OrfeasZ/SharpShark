using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Network.Sockets
{
    internal class Client
    {
        public bool IsConnected { get; set; }

        public event EventHandler<bool> OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler<SharkResponseMessage> OnMessageProcessed;

        private bool m_Connecting;
        private byte[] m_IncomingBuffer;
        private readonly ObjectPool<SocketAsyncEventArgs> m_EventArgsPool;
        private Socket m_Socket;
        private Processor m_Processor;
        private IPEndPoint m_HostEndPoint;

        private readonly Object m_SendLock;

        private UInt16 m_PacketCounter;

        private Dictionary<UInt16, Action<SharkResponseMessage>> m_PacketCallbacks; 

        public Client()
        {
            m_PacketCallbacks = new Dictionary<ushort, Action<SharkResponseMessage>>();
            m_EventArgsPool = new ObjectPool<SocketAsyncEventArgs>(1000);
            m_Connecting = false;
            m_SendLock = new object();
            m_PacketCounter = 0;
        }

        public bool Connect(String p_HostName, UInt16 p_Port)
        {
            if (m_Connecting || IsConnected)
                return false;

            m_PacketCallbacks = new Dictionary<ushort, Action<SharkResponseMessage>>();
            m_PacketCounter = 0;
            m_Processor = new Processor();
            m_Processor.OnMessageProcessed += OnMessageProcessedInternal;

            m_Connecting = true;

            m_IncomingBuffer = new byte[4096];

            IPHostEntry s_Host;

            try
            {
                // This will usually fail when there's no internet connection.
                s_Host = Dns.GetHostEntry(p_HostName);
            }
            catch
            {
                m_Connecting = false;
                return false;
            }

            if (s_Host.AddressList.Length == 0)
            {
                m_Connecting = false;
                return false;
            }

            m_HostEndPoint = new IPEndPoint(s_Host.AddressList[s_Host.AddressList.Length - 1], p_Port);
            m_Socket = new Socket(m_HostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            var s_Args = m_EventArgsPool.Get();
            s_Args.Completed += Connect_Completed;
            s_Args.RemoteEndPoint = m_HostEndPoint;

            if (!m_Socket.ConnectAsync(s_Args))
                Connect_Completed(this, s_Args);

            return true;
        }

        public void Disconnect()
        {
            m_Socket.Disconnect(false);
        }

        private void Connect_Completed(object p_Sender, SocketAsyncEventArgs p_Args)
        {
            m_Connecting = false;

            if (p_Args.SocketError != SocketError.Success)
            {
                IsConnected = false;
                p_Args.Completed -= Connect_Completed;
                p_Args.RemoteEndPoint = null;
                m_EventArgsPool.Put(p_Args);

                if (OnConnected != null)
                    OnConnected(this, false);

                return;
            }

            IsConnected = true;

            var s_Args = m_EventArgsPool.Get();

            // Connect Event
            if (OnConnected != null)
                OnConnected(this, true);

            s_Args.RemoteEndPoint = m_HostEndPoint;
            s_Args.Completed += Receive_Completed;

            PostReceive(s_Args);
        }

        private void PostReceive(SocketAsyncEventArgs p_Args)
        {
            p_Args.SetBuffer(m_IncomingBuffer, 0, m_IncomingBuffer.Length);

            if (!m_Socket.ReceiveAsync(p_Args))
                Receive_Completed(this, p_Args);
        }

        private void Receive_Completed(object p_Sender, SocketAsyncEventArgs p_Args)
        {
            if (p_Args.BytesTransferred == 0 || p_Args.SocketError != SocketError.Success)
            {
                Disconnected(p_Args);
                return;
            }

            m_Processor.ProcessData(m_IncomingBuffer, p_Args.Offset, p_Args.BytesTransferred);

            // TODO: Should we receive again after or before processing the data?
            PostReceive(p_Args);
        }

        public bool SendMessage(SharkMessage p_Message)
        {
            if (p_Message == null)
                return false;

            Trace.WriteLine(String.Format("Sending command: {0}", p_Message.Command));

            try
            {
                var s_SerializedMessage = JsonConvert.SerializeObject(p_Message,
                    new JsonSerializerSettings {ContractResolver = new CamelCaseResolver()});
                s_SerializedMessage += '\n';

                Trace.WriteLine("[C -> S] " + s_SerializedMessage);

                var s_MessageData = Encoding.UTF8.GetBytes(s_SerializedMessage);

                return Send(s_MessageData, 0, s_MessageData.Length);
            }
            catch
            {
                return false;
            }
        }

        public bool SendMessage(SharkMessage p_Message, Action<SharkResponseMessage> p_Callback)
        {
            if (p_Message == null)
                return false;

            if (p_Callback == null)
                return SendMessage(p_Message);

            Trace.WriteLine(String.Format("Sending command: {0}", p_Message.Command));

            if (p_Message.Blackbox == null)
                p_Message.Blackbox = new Dictionary<string, JToken>();

            lock (m_PacketCallbacks)
            {
                var s_PacketID = m_PacketCounter++;
                p_Message.Blackbox.Add("__gspid", s_PacketID);

                if (m_PacketCounter >= 65530)
                    m_PacketCounter = 0;

                m_PacketCallbacks.Add(s_PacketID, p_Callback);
            }

            // TODO: Implement a timeout

            try
            {
                var s_SerializedMessage = JsonConvert.SerializeObject(p_Message,
                    new JsonSerializerSettings { ContractResolver = new CamelCaseResolver() });
                s_SerializedMessage += '\n';

                Trace.WriteLine("[C -> S] " + s_SerializedMessage);

                var s_MessageData = Encoding.UTF8.GetBytes(s_SerializedMessage);

                return Send(s_MessageData, 0, s_MessageData.Length);
            }
            catch
            {
                return false;
            }
        }

        private void OnMessageProcessedInternal(object p_Sender, SharkResponseMessage p_SharkResponseMessage)
        {
            Trace.WriteLine("Received Message:");
            Trace.WriteLine("[S -> C] " + p_SharkResponseMessage);
            Trace.WriteLine("");

            if (p_SharkResponseMessage.Blackbox == null || !p_SharkResponseMessage.Blackbox.ContainsKey("__gspid"))
            {
                if (OnMessageProcessed != null)
                    OnMessageProcessed(this, p_SharkResponseMessage);

                return;
            }

            var s_PacketID = p_SharkResponseMessage.Blackbox["__gspid"].Value<UInt16>();

            Action<SharkResponseMessage> s_Callback;

            lock (m_PacketCallbacks)
            {
                if (!m_PacketCallbacks.ContainsKey(s_PacketID))
                {
                    if (OnMessageProcessed != null)
                        OnMessageProcessed(this, p_SharkResponseMessage);

                    return;
                }

                s_Callback = m_PacketCallbacks[s_PacketID];
                m_PacketCallbacks.Remove(s_PacketID);
            }

            s_Callback(p_SharkResponseMessage);
        }

        private bool Send(byte[] p_Data, int p_Offset, int p_Length)
        {
            if (!m_Socket.Connected)
                return false;

            lock (m_SendLock)
            {

                var s_Args = m_EventArgsPool.Get();
                s_Args.Completed += Send_Completed;
                s_Args.SetBuffer(p_Data, p_Offset, p_Length);

                try
                {
                    if (!m_Socket.SendAsync(s_Args))
                        Send_Completed(this, s_Args);

                    return true;
                }
                catch (ObjectDisposedException)
                {
                    s_Args.UserToken = null;
                    s_Args.Completed -= Send_Completed;
                    m_EventArgsPool.Put(s_Args);

                    return false;
                }
            }
        }

        private void Send_Completed(object p_Sender, SocketAsyncEventArgs p_Args)
        {
            // NOTE: Is there a chance not all data is sent?
            p_Args.UserToken = null;
            p_Args.Completed -= Send_Completed;
            m_EventArgsPool.Put(p_Args);
        }

        private void Disconnected(SocketAsyncEventArgs p_Args)
        {
            p_Args.Completed -= Receive_Completed;
            m_EventArgsPool.Put(p_Args);

            IsConnected = false;

            if (OnDisconnected != null)
                OnDisconnected(this, null);
        }
    }
}
