using System;
using System.Collections.Generic;
using GS.Lib.Enums;
using GS.Lib.Models;
using GS.Lib.Util;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void PingMaster()
        {
            // TODO: Implement timeout timer
            BroadcastMessageToSelf("masterPing");
        }

        private MasterStatus GetPrivateStatus()
        {
            return new MasterStatus()
            {
                UUID = UID,
                IsBroadcasting = Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Broadcasting,
                CurrentBroadcast = Library.Broadcast.CurrentBroadcast,
                CurrentlyPlayingSong = false,
                LastMouseMove = DateTime.UtcNow.ToUnixTimestamp() * 1000
            };
        }

        private void PromoteSelfToMaster(String p_Reason = "unknown")
        {
            if (Library.User.Data == null || Library.User.Data.UserID == 0)
                return;

            LoggedInMaster = GetPrivateStatus();
            LoggedInMaster.LastUpdate = DateTime.UtcNow.ToUnixTimestamp() * 1000;
            LoggedInMaster.Reason = p_Reason;

            UpdateCurrentStatus();

            BroadcastMessageToSelf(LoggedInMaster, "masterPromotion");
        }

        internal void ReEvaluateMastershipAndUpdate()
        {
            if (Library.User.Data == null || Library.User.Data.UserID == 0)
                return;

            if (LoggedInMaster != null && LoggedInMaster.UUID == UID)
            {
                // TODO: Check for change in playing state and PromoteSelfToMaster("evaluate_and_update_self").
                UpdateCurrentStatus();
                return;
            }

            // TODO: Handle idle updates
        }
    }
}
