using System;
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
                IsBroadcasting = Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Broadcasting ? 1 : 0,
                CurrentBroadcast = Library.Broadcast.ActiveBroadcastID,
                CurrentlyPlayingSong = 0,
                LastMouseMove = DateTime.UtcNow.ToUnixTimestampMillis()
            };
        }

        private void PromoteSelfToMaster(String p_Reason = "unknown")
        {
            if (Library.User.Data == null || Library.User.Data.UserID == 0)
                return;

            LoggedInMaster = GetPrivateStatus();
            LoggedInMaster.LastUpdate = DateTime.UtcNow.ToUnixTimestampMillis();
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
                // TODO: Also check for songplaying
                if (LoggedInMaster.CurrentBroadcast != Library.Broadcast.ActiveBroadcastID ||
                    (LoggedInMaster.IsBroadcasting != 1) !=
                    (Library.Broadcast.CurrentBroadcastStatus == BroadcastStatus.Broadcasting))
                {
                    PromoteSelfToMaster("evaluate_and_update_self");
                    return;
                }

                UpdateCurrentStatus();
                return;
            }

            // TODO: Handle idle updates
        }
    }
}
