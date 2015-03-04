using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using GS.Lib.Events;
using GS.Lib.Models;
using GS.Lib.Util;

namespace GS.Lib.Components
{
    internal class RemoraComponent : SharkComponent
    {
        public String ControlChannel { get; set; }

        public String TestingChannel { get; set; }

        private readonly Queue<Object> m_QueuedMessages;

        private bool m_Created;
        private bool m_PendingCreation;

        public RemoraComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            m_QueuedMessages = new Queue<object>();

            m_Created = false;
            m_PendingCreation = false;
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
            if (!m_Created)
                return false;

            if (!p_Event.Success)
                return false;

            if (p_Event.Blackbox == null)
                return false;

            if (!p_Event.Blackbox.ContainsKey("source"))
                return false;

            if (p_Event.Blackbox["source"] as String != "takeoverControlChannel")
                return false;

            Library.Chat.PublishToChannels(new List<string>() { TestingChannel }, new Dictionary<String, String>()
            {
                { "setup", ControlChannel }
            });

            return true;
        }

        internal bool HandlePublish(SubUpdateEvent p_Event)
        {
            if (p_Event.Sub != ControlChannel)
                return false;

            if (p_Event.Value == null)
                return true;

            if (!m_Created && !m_PendingCreation)
            {
                if (p_Event.ID.ContainsKey("sudo") && (bool) p_Event.ID["sudo"] &&
                    p_Event.Value.ContainsKey("available"))
                {
                    m_PendingCreation = true;

                    var s_Params = Library.Broadcast.GetParamsForRemora();
                    s_Params.Add("queueChannel", ControlChannel);
                    s_Params.Add("isGhostBroadcast", true);

                    Library.Chat.PublishToChannels(new List<string>() { TestingChannel }, new Dictionary<String, Object>()
                    {
                        { "setupQueue", ControlChannel },
                        { "ownerID", p_Event.Value["available"] },
                        { "queue", s_Params }
                    });
                }

                return true;
            }

            if (!p_Event.Value.ContainsKey("action"))
                return true;

            if (p_Event.Value["action"] as String == "setupQueueSuccess")
            {
                if (!p_Event.Value.ContainsKey("broadcast") || p_Event.Value["broadcast"] == null)
                    return true;

                m_Created = true;
                m_PendingCreation = false;

                while (m_QueuedMessages.Count > 0)
                    Library.Chat.PublishToChannels(new List<string>() { ControlChannel }, m_QueuedMessages.Dequeue());

                return true;
            }

            return true;
        }
    }
}
