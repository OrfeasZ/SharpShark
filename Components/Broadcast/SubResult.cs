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

            var s_PrivateChannel = Library.Chat.GetChatChannel(ActiveBroadcastID);
            var s_PublicChannel = Library.Chat.GetChatChannel(ActiveBroadcastID, true);

            if (s_Event.Channels.ContainsKey(s_PrivateChannel) && s_Event.Channels.ContainsKey(s_PublicChannel))
            {
                if (m_ConnectedToChatChannels)
                    return;

                m_ConnectedToChatChannels = true;

                if (s_Event.Blackbox.ContainsKey("source") && (s_Event.Blackbox["source"].ToObject<String>() == "startBroadcast" ||
                                                     s_Event.Blackbox["source"].ToObject<String>() == "joinBroadcast" ||
                                                     s_Event.Blackbox["source"].ToObject<String>() == "resumeBroadcast" ||
                                                     s_Event.Blackbox["source"].ToObject<String>() == "takeOverBroadcast" ||
                                                     s_Event.Blackbox["source"].ToObject<String>() == "startBroadcast"))
                {
                    if (s_Event.Blackbox["source"].ToObject<String>() == "startBroadcast")
                    {
                        UpdateChatServer(null, "startBroadcastUpdate",
                            s_Event.Blackbox.ContainsKey("token") ? s_Event.Blackbox["token"].ToObject<Dictionary<String, Object>>() : null);
                    }
                    else if (s_Event.Blackbox["source"].ToObject<String>() == "takeOverBroadcast")
                    {
                        InitializeForTakeOverEx(s_Event.Blackbox.ContainsKey("pendingNewBroadcastID")
                            ? s_Event.Blackbox["pendingNewBroadcastID"].ToObject<String>()
                            : null);
                    }
                    else
                    {
                        FinalizeActivation(
                            s_Event.Blackbox.ContainsKey("isBroadcasting") && s_Event.Blackbox["isBroadcasting"].ToObject<bool>(), new List<string>(), 
                            s_Event.Blackbox["source"].ToObject<String>(),
                            s_Event.Blackbox.ContainsKey("token") ? s_Event.Blackbox["token"].ToObject<Dictionary<String, Object>>() : null);
                    }
                }
            }
        }
    }
}
