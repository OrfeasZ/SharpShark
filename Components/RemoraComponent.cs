using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GS.Lib.Events;
using GS.Lib.Models;
using GS.Lib.Util;

namespace GS.Lib.Components
{
    internal class RemoraComponent : SharkComponent
    {
        public String ControlChannel { get; set; }

        private readonly Queue<Object> m_QueuedMessages;

        private bool m_Created;

        public RemoraComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            m_QueuedMessages = new Queue<object>();
            m_Created = false;
        }

        internal void JoinControlChannels()
        {
            if (Library.User.Data == null)
                return;

            m_QueuedMessages.Clear();

            using (var s_MD5 = MD5.Create())
            {
                var s_Hash = s_MD5.ComputeHash(Encoding.ASCII.GetBytes(Library.User.Data.UserID.ToString() + (DateTime.UtcNow.ToUnixTimestamp() * 1000).ToString()));
                ControlChannel = BitConverter.ToString(s_Hash).Replace("-", "").ToLower();
            }

            Library.Chat.JoinChannels(new List<object>()
            {
                new Dictionary<String, Object>()
                {
                    { "sub", ControlChannel },
                    { "overwrite_params", false },
                    { "create_when_dne", true }
                }
            });

            Library.Chat.SetSubscriptionParameters(ControlChannel, new Dictionary<string, object>()
            {
                { "sub_alert", true },
                { "owners", new List<UserIDData>() { new UserIDData(Library.User.Data.UserID) }},
            },
            new Dictionary<string, object>()
            {
                { "source", "takeoverControlChannel" }
            });
        }

        internal void Send(Object p_Message)
        {
            if (!m_Created)
            {
                m_QueuedMessages.Enqueue(p_Message);
                return;
            }

            Library.Chat.PublishToChannels(new List<string>() { ControlChannel }, p_Message);
        }

        internal bool HandleSetResult(SetResultEvent p_Event)
        {
            
        }

        internal bool HandlePublish(PubResultEvent p_Event)
        {
            
        }
    }
}
