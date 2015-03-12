using GS.Lib.Events;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private void HandleSubUpdate(SharkEvent p_Event)
        {
            var s_Event = p_Event.As<SubUpdateEvent>();

            if (s_Event.Sub != Library.Chat.GetChatChannel(ActiveBroadcastID) &&
                s_Event.Sub != Library.Chat.GetChatChannel(ActiveBroadcastID, true))
            {
                if (s_Event.Type == "publish")
                    Library.Remora.HandlePublish(s_Event);

                return;
            }

            // TODO: Implement
        }
    }
}
