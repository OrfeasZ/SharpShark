using System;
using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Responses;
using GS.Lib.Util;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandleSelfMessages(PublishResponse p_Message)
        {
            if (UID != p_Message.Publish.ID["uid"] as String || p_Message.Publish.Value == null)
                return;

            var s_Params = p_Message.Publish.Value["params"] as Dictionary<String, Object>;
            var s_UID = p_Message.Publish.ID["uid"] as String;

            switch (p_Message.Publish.Value["type"] as String)
            {
                case "statusRequest":
                    BroadcastMessageToSelf(GetCurrentPublicStatus(), "statusReply");
                    break;

                case "masterPing":
                    if (LoggedInMaster.UUID == UID)
                        BroadcastMessageToSelf(GetPrivateStatus(), "masterPong");

                    break;

                case "masterPong":
                    UpdateLoggedInMaster(s_Params);
                    break;

                case "masterPromotion":
                    UpdateLoggedInMaster(s_Params);
                    break;

                case "statusUpdate":
                    if (LoggedInMaster != null && LoggedInMaster.UUID == s_UID)
                        LoggedInMaster.LastUpdate = DateTime.UtcNow.ToUnixTimestampMillis();

                    break;
            }

        }

        private void UpdateLoggedInMaster(Dictionary<String, Object> p_Attributes)
        {
            if (p_Attributes == null)
                return;

            LoggedInMaster = new MasterStatus();

            Object s_UUID;
            if (p_Attributes.TryGetValue("uuid", out s_UUID))
                LoggedInMaster.UUID = s_UUID as String;

            Object s_LastMouseMove;
            if (p_Attributes.TryGetValue("lastMouseMove", out s_LastMouseMove))
                LoggedInMaster.LastMouseMove = (long) s_LastMouseMove;

            Object s_CurrentBroadcast;
            if (p_Attributes.TryGetValue("currentBroadcast", out s_CurrentBroadcast))
                LoggedInMaster.CurrentBroadcast = s_CurrentBroadcast as String;

            Object s_Broadcasting;
            if (p_Attributes.TryGetValue("isBroadcasting", out s_Broadcasting))
                LoggedInMaster.IsBroadcasting = (int) s_Broadcasting;

            Object s_CurrentlyPlayingSong;
            if (p_Attributes.TryGetValue("currentlyPlayingSong", out s_CurrentlyPlayingSong))
                LoggedInMaster.CurrentlyPlayingSong = (int) s_CurrentlyPlayingSong;

            Object s_Reason;
            if (p_Attributes.TryGetValue("reason", out s_Reason))
                LoggedInMaster.Reason = s_Reason as String;

            LoggedInMaster.LastUpdate = DateTime.UtcNow.ToUnixTimestampMillis();
        }
    }
}
