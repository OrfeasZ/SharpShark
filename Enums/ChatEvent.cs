﻿namespace GS.Lib.Enums
{
    public enum ChatEvent : int
    {
        ChatError,
        RemoteResult,
        PubResult,
        ConnectionError,
        NewMaster,
        Open,
        BroadcastDarkLaunch,
        SubUpdate,
        DataReceived,
        Close,
        Identified,
        SubResult,
        ChatReady,
        ListResult,
        UserChanged,
        Error,
        QueueTransferRequest,
        SelfChannelJoinAlert,
        NeedAppDataRefresh,
        DataParseError,
        ChannelJoinError,
        ConnectionDrop,
        JoinAlert,
        SetResult,
        Message
    }
}
