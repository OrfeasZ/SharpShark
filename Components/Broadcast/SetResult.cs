using System;
using System.Collections.Generic;
using GS.Lib.Events;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private void HandleSetResult(SharkEvent p_Event)
        {
            var s_Event = p_Event.As<SetResultEvent>();


        }
    }
}
