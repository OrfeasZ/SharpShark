using System;
using System.Collections.Generic;
using GS.Lib.Events;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private bool m_ConnectedToChatChannels;

        private void HandleSubResult(SharkEvent p_Event)
        {
            var s_Event = p_Event.As<SubResultEvent>();

            if (String.IsNullOrWhiteSpace(ActiveBroadcastID))
                return;
        }
    }
}
