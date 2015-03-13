using System;
using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Responses;
using GS.Lib.Util;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandleSelfMessages(PublishResponse p_Message)
        {
            if (UID != p_Message.Publish.ID["uid"].ToObject<String>() || p_Message.Publish.Value == null)
                return;

            var s_Params = p_Message.Publish.Value["params"].ToObject<Dictionary<String, JToken>>();
            var s_UID = p_Message.Publish.ID["uid"].ToObject<String>();

            switch (p_Message.Publish.Value["type"].ToObject<String>())
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

        private void UpdateLoggedInMaster(Dictionary<String, JToken> p_Attributes)
        {
            if (p_Attributes == null)
                return;

            LoggedInMaster = new MasterStatus();

            JToken s_UUID;
            if (p_Attributes.TryGetValue("uuid", out s_UUID))
                LoggedInMaster.UUID = s_UUID.Value<String>();

            JToken s_LastMouseMove;
            if (p_Attributes.TryGetValue("lastMouseMove", out s_LastMouseMove))
                LoggedInMaster.LastMouseMove = s_LastMouseMove.Value<Int64>();

            JToken s_CurrentBroadcast;
            if (p_Attributes.TryGetValue("currentBroadcast", out s_CurrentBroadcast))
                LoggedInMaster.CurrentBroadcast = s_CurrentBroadcast.Value<String>();

            JToken s_Broadcasting;
            if (p_Attributes.TryGetValue("isBroadcasting", out s_Broadcasting))
                LoggedInMaster.IsBroadcasting = s_Broadcasting.Value<int>();

            JToken s_CurrentlyPlayingSong;
            if (p_Attributes.TryGetValue("currentlyPlayingSong", out s_CurrentlyPlayingSong))
                LoggedInMaster.CurrentlyPlayingSong = s_CurrentlyPlayingSong.Value<int>();

            JToken s_Reason;
            if (p_Attributes.TryGetValue("reason", out s_Reason))
                LoggedInMaster.Reason = s_Reason.Value<String>();

            LoggedInMaster.LastUpdate = DateTime.UtcNow.ToUnixTimestampMillis();
        }
    }
}
