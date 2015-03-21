using GS.Lib.Enums;

namespace GS.Lib.Events
{
    public class PlaybackStatusUpdateEvent : SharkEvent
    {
        public PlaybackStatus Status { get; set; }
    }
}
