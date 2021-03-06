﻿namespace GS.Lib.Enums
{
    public enum ClientEvent
    {
        AuthenticationFailed,
        Authenticated,
        BroadcastCreationFailed,
        BroadcastCreated,
        ChatMessage,
        SongVote,
        SongPlaying,
        QueueUpdated,
        SongSuggestion,
        SongSuggestionRemoved,
        SongSuggestionRejected,
        SongSuggestionApproved,
        PendingDestruction,
        ComplianceIssue,
        Connected,
        Disconnected,
        UserJoinedBroadcast,
        UserLeftBroadcast,
        PlaybackStatusUpdate
    }
}
