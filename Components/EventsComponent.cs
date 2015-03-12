using System;
using System.Collections.Generic;
using GS.Lib.Events;

namespace GS.Lib.Components
{
    public abstract class EventsComponent : SharkComponent
    {
        private readonly Dictionary<int, List<Action<SharkEvent>>> m_EventHandlers; 

        protected EventsComponent(SharpShark p_Library) 
            : base(p_Library)
        {
            m_EventHandlers = new Dictionary<int, List<Action<SharkEvent>>>();
        }

        internal void RegisterEventHandler(int p_Event, Action<SharkEvent> p_Callback)
        {
            if (!m_EventHandlers.ContainsKey(p_Event))
                m_EventHandlers.Add(p_Event, new List<Action<SharkEvent>>());

            if (m_EventHandlers[p_Event].Contains(p_Callback))
                return;

            m_EventHandlers[p_Event].Add(p_Callback);
        }

        internal void RemoveEventHandler(int p_Event, Action<SharkEvent> p_Callback)
        {
            if (!m_EventHandlers.ContainsKey(p_Event))
                return;

            m_EventHandlers[p_Event].Remove(p_Callback);
        }

        internal void RemoveEventHandlers(int p_Event)
        {
            if (!m_EventHandlers.ContainsKey(p_Event))
                return;

            m_EventHandlers[p_Event].Clear();
        }

        internal void DispatchEvent(int p_Event, SharkEvent p_Data)
        {
            List<Action<SharkEvent>> s_Handlers;
            if (!m_EventHandlers.TryGetValue(p_Event, out s_Handlers))
                return;

            foreach (var s_Handler in s_Handlers)
                s_Handler(p_Data);
        }
    }
}
