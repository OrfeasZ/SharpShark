using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GS.Lib.Network.Sockets.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        private Object m_SendLock;

        public Client()
        {
            m_EventArgsPool = new ObjectPool<SocketAsyncEventArgs>(1000);
            m_Connecting = false;
            m_SendLock = new object();
        }

        public bool Connect(String p_HostName, UInt16 p_Port)
        {
            if (m_Connecting || IsConnected)
                return false;

            m_Processor = new Processor();
            m_Processor.OnMessageProcessed += OnMessageProcessed;

            m_Connecting = true;

            m_IncomingBuffer = new byte[4096];

            var s_Host = Dns.GetHostEntry(p_HostName);

            if (s_Host.AddressList.Length == 0)
            {
                m_Connecting = false;
                return false;
            }

            var s_Endpoint = new IPEndPoint(s_Host.AddressList[s_Host.AddressList.Length - 1], p_Port);
            m_Socket = new Socket(s_Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            var s_Args = m_EventArgsPool.Get();
            s_Args.Completed += Connect_Completed;
            s_Args.RemoteEndPoint = s_Endpoint;

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

            Debug.WriteLine(String.Format("Sending command: {0}", p_Message.Command));

            try
            {
                var s_SerializedMessage = JsonConvert.SerializeObject(p_Message,
                    new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
                s_SerializedMessage += '\n';

                Debug.WriteLine("[C -> S] " + s_SerializedMessage);

                var s_MessageData = Encoding.UTF8.GetBytes(s_SerializedMessage);

                return Send(s_MessageData, 0, s_MessageData.Length);
            }
            catch
            {
                return false;
            }
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
