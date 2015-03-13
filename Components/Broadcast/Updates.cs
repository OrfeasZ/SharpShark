using System;
using System.Collections.Generic;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private static readonly Dictionary<String, Object> m_ChatChannelKeyMap = new Dictionary<String, Object>
        {
            { "chatEnabled", "c" },
            { "suggestionsEnabled", "sge" },
            { "playbackStatus", "s" },
            { "history", "h" },
            { "owners", "owners" },
            { "publishers", "publishers" },
            { "tags", "tags" },
            { "bannedUserIDs", "banned" },
            { "suggestions", "sg" },
            { "suggestionsChanges", "sgc" },
            { "Name", "n" },
            { "Tag", "t" },
            { "Description", "d" },
            { "isPlaying", "py" },
            { "Image", "i" },
            { "Privacy", "p" },
            { "totalListens", "tl" },
            { "ownerSubscribed", "owner_subscribed" },
            { "pendingNewOwnerUserDetails", "pndOwner" },
            { "broadcasterVersion", "v" },
            { "vipUsers", "vp" },
            { "liveURL", "liveURL" },
            { "broadcastIncrement", "idSuffix" },
            { "mobileCompliant", "se" }
        };

        private static readonly Dictionary<String, object> m_ChatPrivateChannelExtraProperties = new Dictionary<string, object>()
        {
            { "sub_alert", true },
            { "unsub_alert", true },
            { "subscribers_count_delay", true }
        };

        private static readonly Dictionary<String, object> m_ChatPublicChannelExtraProperties = new Dictionary<string, object>()
        {
            { "publishers", "global" },
            { "ban_groups", new []{ "bcast_chat"} },
            { "store_latest", true }
        };
    }
}
