using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;
using GS.Lib.Util;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    internal class RemoraComponent : SharkComponent
    {
        public String ControlChannel { get; set; }

        public String Channel { get; set; }

        private readonly Queue<Object> m_QueuedMessages;

        private bool m_Created;
        private bool m_PendingCreation;

        private Timer m_PingTimer;

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
                var s_Hash = s_MD5.ComputeHash(Encoding.ASCII.GetBytes(Library.User.Data.UserID.ToString() + (DateTime.UtcNow.ToUnixTimestampMillis()).ToString()));
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
            }, p_Callback: p_Message =>
            {
                Library.Chat.SetSubscriptionParameters(ControlChannel, new Dictionary<string, object>()
                {
                    { "sub_alert", true },
                    { "owners", new List<UserIDData>() { new UserIDData(Library.User.Data.UserID) }},
                }, new Dictionary<string, JToken>(), p_Callback: HandleSetResult);
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

        internal void HandleSetResult(SharkResponseMessage p_Message)
        {
            if (m_Created)
                return;

            var s_Message = p_Message.As<SuccessResponse<String>>();

            if (s_Message == null)
                return;

            if (s_Message.Success == null || s_Message.Success != "set")
                return;

            Library.Chat.PublishToChannels(new List<string>() { Channel }, new Dictionary<String, String>()
            {
                { "setup", ControlChannel }
            });
        }

        internal bool HandlePublish(SubUpdateEvent p_Event)
        {
            if (p_Event.Sub != ControlChannel)
                return false;

            if (p_Event.Value == null)
                return true;

            if (!m_Created && !m_PendingCreation)
            {
                if (p_Event.ID.ContainsKey("sudo") && (bool)p_Event.ID["sudo"] &&
                    p_Event.Value.ContainsKey("available"))
                {
                    m_PendingCreation = true;

                    var s_Params = Library.Broadcast.GetParamsForRemora();
                    s_Params.Add("queueChannel", ControlChannel);

                    Library.Chat.PublishToChannels(new List<string>() { Channel }, new Dictionary<String, Object>()
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

            if (p_Event.Value["action"].Value<String>() == "setupQueueFailed")
            {
                m_PendingCreation = false;
                m_Created = false;

                Library.Broadcast.CurrentBroadcastStatus = BroadcastStatus.Idle;
                Library.Broadcast.ActiveBroadcastID = null;

                Library.DispatchEvent(ClientEvent.BroadcastCreationFailed, null);
                return true;
            }

            if (p_Event.Value["action"].Value<String>() == "setupQueueSuccess")
            {
                if (!p_Event.Value.ContainsKey("broadcast") || p_Event.Value["broadcast"] == null)
                    return true;

                m_Created = true;
                m_PendingCreation = false;

                var s_Broadcast = p_Event.Value["broadcast"];

                Library.Broadcast.ActiveBroadcastID = s_Broadcast["BroadcastID"].Value<String>();
                Library.Broadcast.CurrentBroadcastName = s_Broadcast["Name"].Value<String>();
                Library.Broadcast.CurrentBroadcastStatus = BroadcastStatus.Broadcasting;

                while (m_QueuedMessages.Count > 0)
                    Library.Chat.PublishToChannels(new List<string>() { ControlChannel }, m_QueuedMessages.Dequeue());

                Library.Chat.UpdateCurrentStatus();

                Library.Chat.SubscribeToBroadcastStatuses();

                return true;
            }

            return true;
        }

        internal void StartPing()
        {
            if (m_PingTimer != null)
                m_PingTimer.Dispose();

            m_PingTimer = new Timer
            {
                Interval = 45000,
                AutoReset = true
            };

            m_PingTimer.Elapsed += Ping;

            m_PingTimer.Start();
        }

        internal void StopPing()
        {
            if (m_PingTimer == null)
                return;

            m_PingTimer.Dispose();
            m_PingTimer = null;
        }

        internal void Ping(object p_Sender, ElapsedEventArgs p_Args)
        {
            Send(new Dictionary<String, Object>()
            {
                { "action", "ping" },
                { "idle", false },
                { "away", false }
            });
        }
    }
}
