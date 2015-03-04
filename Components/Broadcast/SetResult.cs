using System;
using System.Collections.Generic;
using System.Diagnostics;
using GS.Lib.Events;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private void HandleSetResult(SharkEvent p_Event)
        {
            var s_Event = p_Event.As<SetResultEvent>();

            if (Library.Remora.HandleSetResult(s_Event))
                ;//return;

            if (s_Event.Blackbox == null || !s_Event.Blackbox.ContainsKey("source"))
                return;

            switch (s_Event.Blackbox["source"] as String)
            {
                case "startBroadcastUpdate":
                    if (s_Event.Success && s_Event.Blackbox.ContainsKey("channelType") && s_Event.Blackbox["channelType"] as String == "public")
                    {
                        Debug.WriteLine("Finalizing activation.");
                        FinalizeActivation(true, new List<string>(), "startBroadcast", s_Event.Blackbox.ContainsKey("token") ? s_Event.Blackbox["token"] : null);
                        break;
                    }

                    break;
            }

        }
    }
}
