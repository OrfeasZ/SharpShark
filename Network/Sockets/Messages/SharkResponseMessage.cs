using System;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages
{
    internal sealed class SharkResponseMessage
    {
        public String Command { get; set; }

        private String m_MessageData;

        public void SetMessageData(String p_MessageData)
        {
            m_MessageData = p_MessageData;
        }

        public T As<T>()
            where T : SharkMessage
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(m_MessageData);
            }
            catch
            {
                return null;
            }
        }
    }
}
