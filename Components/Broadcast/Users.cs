using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        public List<Int64> BannedUserIDs { get; private set; }

        public List<Int64> OwnerUserIDs { get; set; } 

        private List<UserIDData> GetChatVariableBannedUsers()
        {
            return BannedUserIDs.Select(p_User => new UserIDData(p_User)).ToList();
        }

        private List<Dictionary<String, Object>> GetChatVariableVIPUsers()
        {
            // TODO: Implement
            return new List<Dictionary<string, object>>();
        }

        private List<Dictionary<String, Object>> GetChatVariablePublishers()
        {
            // TODO: Implement
            return new List<Dictionary<string, object>>();
        }

        private List<UserIDData> GetChatVariableOwners()
        {
            return OwnerUserIDs.Select(p_User => new UserIDData(p_User)).ToList();
        }

        private void GetListeners()
        {
            Library.Chat.GetSubscriberList(Library.Chat.GetChatChannel(ActiveBroadcastID), true, new Dictionary<string, object>()
            {
                { "source", "broadcast" },
                { "broadcastID", ActiveBroadcastID }
            });
        }
    }
}
