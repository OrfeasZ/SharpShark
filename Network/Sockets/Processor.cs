using System;
using System.Diagnostics;
using System.Text;
using GS.Lib.Network.Sockets.Messages;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets
{
    internal class Processor
    {
        public EventHandler<SharkResponseMessage> OnMessageProcessed; 

        private byte[] m_IncomingData;

        public Processor()
        {
            m_IncomingData = new byte[0];
        }

        public void ProcessData(byte[] p_Data, int p_Offset, int p_Length)
        {
            var s_Data = new byte[p_Length];
            Buffer.BlockCopy(p_Data, p_Offset, s_Data, 0, p_Length);
            ProcessData(s_Data);
        }

        public void ProcessData(byte[] p_Data)
        {
            // Append the new data.
            if (p_Data != null)
            {
                var s_NewData = new byte[m_IncomingData.Length + p_Data.Length];
                Buffer.BlockCopy(m_IncomingData, 0, s_NewData, 0, m_IncomingData.Length);
                Buffer.BlockCopy(p_Data, 0, s_NewData, m_IncomingData.Length, p_Data.Length);
                m_IncomingData = s_NewData;
            }

            var s_SeparatorIndex = Array.IndexOf(m_IncomingData, (byte) 0x0A);

            if (s_SeparatorIndex == -1)
                return;

            // Get our message data.
            var s_MessageData = new byte[s_SeparatorIndex];
            Buffer.BlockCopy(m_IncomingData, 0, s_MessageData, 0, s_SeparatorIndex);

            // Parse the message.
            var s_MessageStringData = Encoding.UTF8.GetString(s_MessageData);

            try
            {
                var s_Message = JsonConvert.DeserializeObject<SharkResponseMessage>(s_MessageStringData);
                s_Message.SetMessageData(s_MessageStringData);

                if (OnMessageProcessed != null)
                    OnMessageProcessed(this, s_Message);
            }
            catch
            {
                Debug.WriteLine("Failed to deserialize message: {0}", s_MessageStringData);
            }

            if (m_IncomingData.Length > s_SeparatorIndex + 1)
            {
                var s_NewData = new byte[m_IncomingData.Length - (s_SeparatorIndex + 1)];
                Buffer.BlockCopy(m_IncomingData, s_SeparatorIndex + 1, s_NewData, 0, s_NewData.Length);
                m_IncomingData = s_NewData;

                // Process remaining data.
                ProcessData(null);
                return;
            }

            m_IncomingData = new byte[0];
        }

        
    }
}
