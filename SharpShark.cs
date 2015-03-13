using System;
using System.Collections.Generic;
using GS.Lib.Components;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Network.HTTP;

namespace GS.Lib
{
    public class SharpShark
    {
        internal RequestDispatcher RequestDispatcher { get; private set; }

        public UserComponent User { get; private set; }
        public ChatComponent Chat { get; private set; }
        public BroadcastComponent Broadcast { get; private set; }
        public SearchComponent Search { get; private set; }
        public SongsComponent Songs { get; private set; }
        public QueueComponent Queue { get; private set; }
        internal RemoraComponent Remora { get; private set; }

        public String BaseURL { get; private set; }

        internal Int64 TimeDifference { get; set; }

        private readonly Dictionary<ClientEvent, List<Action<SharkEvent>>> m_EventHandlers; 

        public SharpShark(String p_SecretKey)
        {
            BaseURL = "http://grooveshark.com";
            RequestDispatcher = new RequestDispatcher(this, p_SecretKey);
            m_EventHandlers = new Dictionary<ClientEvent, List<Action<SharkEvent>>>();

            InitComponents();
            RegisterEventHandlers();
        }

        public SharpShark(String p_SecretKey, String p_BaseURL)
        {
            BaseURL = p_BaseURL;
            RequestDispatcher = new RequestDispatcher(this, p_SecretKey);

            InitComponents();
            RegisterEventHandlers();
        }

        private void InitComponents()
        {
            User = new UserComponent(this);
            Chat = new ChatComponent(this);
            Broadcast = new BroadcastComponent(this);
            Search = new SearchComponent(this);
            Songs = new SongsComponent(this);
            Queue = new QueueComponent(this);
            Remora = new RemoraComponent(this);
        }

        private void RegisterEventHandlers()
        {
            User.RegisterEventHandlers();
            Chat.RegisterEventHandlers();
            Broadcast.RegisterEventHandlers();
            Search.RegisterEventHandlers();
            Songs.RegisterEventHandlers();
            Queue.RegisterEventHandlers();
            Remora.RegisterEventHandlers();
        }

        public void RegisterEventHandler(ClientEvent p_Event, Action<SharkEvent> p_Callback)
        {
            lock (m_EventHandlers)
            {
                if (!m_EventHandlers.ContainsKey(p_Event))
                    m_EventHandlers.Add(p_Event, new List<Action<SharkEvent>>());

                if (m_EventHandlers[p_Event].Contains(p_Callback))
                    return;

                m_EventHandlers[p_Event].Add(p_Callback);
            }
        }

        public void RemoveEventHandler(ClientEvent p_Event, Action<SharkEvent> p_Callback)
        {
            lock (m_EventHandlers)
            {
                if (!m_EventHandlers.ContainsKey(p_Event))
                    return;

                m_EventHandlers[p_Event].Remove(p_Callback);
            }
        }

        public void RemoveEventHandlers(ClientEvent p_Event)
        {
            lock (m_EventHandlers)
            {
                if (!m_EventHandlers.ContainsKey(p_Event))
                    return;

                m_EventHandlers[p_Event].Clear();
            }
        }

        internal void DispatchEvent(ClientEvent p_Event, SharkEvent p_Data)
        {
            List<Action<SharkEvent>> s_Handlers;
            if (!m_EventHandlers.TryGetValue(p_Event, out s_Handlers))
                return;

            foreach (var s_Handler in s_Handlers)
                s_Handler(p_Data);
        }
    }
}
