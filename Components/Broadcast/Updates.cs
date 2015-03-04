using System;
using System.Collections.Generic;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Network.Sockets.Messages.Requests;

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

        private void UpdateChatServer(Dictionary<String, Object> p_ChangedProps = null, String p_Source = null, Dictionary<String, Object> p_Token = null)
        {
            if (!m_ConnectedToChatChannels)
                return;

            if (String.IsNullOrWhiteSpace(p_Source))
                p_Source = "broadcastUpdateServer";

            var s_PrivateSubParams = new Dictionary<String, Object>(); // loc6
            var s_PublicSubParams = new Dictionary<String, Object>(); // loc4

            int s_PublicInstances = 0; // loc5
            int s_PrivateInstances = 0; // loc7

            if (p_ChangedProps == null)
            {
                p_ChangedProps = new Dictionary<string, object>(m_ChatChannelKeyMap);

                ++s_PrivateInstances;
                foreach (var s_Pair in m_ChatPrivateChannelExtraProperties)
                    s_PrivateSubParams.Add(s_Pair.Key, s_Pair.Value);

                ++s_PublicInstances;
                foreach (var s_Pair in m_ChatPublicChannelExtraProperties)
                    s_PublicSubParams.Add(s_Pair.Key, s_Pair.Value);
            }

            foreach (var s_Pair in p_ChangedProps)
            {
                var s_PropertyKey = m_ChatChannelKeyMap[s_Pair.Key] as String;

                switch (s_Pair.Key)
                {
                    case "chatEnabled":
                        s_PublicSubParams.Add(s_PropertyKey, ChatEnabled);
                        ++s_PublicInstances;
                        continue;
                        
                    case "suggestions":
                        s_PublicSubParams.Add(m_ChatChannelKeyMap["suggestionsChanges"] as String, m_SuggestionChanges);

                        if (m_SuggestionChanges.Count == 0)
                            s_PublicSubParams.Add(s_PropertyKey, GetChatVariableSuggestions());

                        ++s_PublicInstances;
                        continue;

                    case "bannedUserIDs":
                        s_PublicSubParams.Add(s_PropertyKey, GetChatVariableBannedUsers());
                        ++s_PublicInstances;
                        continue;

                    case "suggestionsEnabled":
                        s_PublicSubParams.Add(s_PropertyKey, SuggestionsEnabled);
                        ++s_PublicInstances;
                        continue;

                    case "vipUsers":
                        s_PublicSubParams.Add(s_PropertyKey, GetChatVariableVIPUsers());
                        ++s_PublicInstances;
                        continue;

                    case "playbackStatus":
                        s_PrivateSubParams.Add(s_PropertyKey, GetChatVariableStatus());
                        ++s_PrivateInstances;
                        continue;

                    case "emptyPlaybackStatus":
                        s_PrivateSubParams.Add(m_ChatChannelKeyMap["playbackStatus"] as String, new object());
                        ++s_PrivateInstances;
                        continue;

                    case "history":
                    case "historyAdded":
                        s_PrivateSubParams.Add(m_ChatChannelKeyMap["history"] as String, GetChatVariableHistory());
                        ++s_PrivateInstances;
                        continue;

                    case "tags":
                        s_PrivateSubParams.Add(s_PropertyKey, GetChatVariableTags());
                        ++s_PrivateInstances;
                        continue;

                    case "publishers":
                        s_PrivateSubParams.Add(s_PropertyKey, GetChatVariablePublishers());
                        ++s_PrivateInstances;
                        continue;

                    case "broadcastIncrement":
                        // TODO: Implement
                        s_PrivateSubParams.Add(s_PropertyKey, null);
                        ++s_PrivateInstances;
                        continue;

                    case "Name":
                        s_PrivateSubParams.Add(s_PropertyKey, CurrentBroadcastName);
                        ++s_PrivateInstances;
                        continue;

                    case "Description":
                        s_PrivateSubParams.Add(s_PropertyKey, CurrentBroadcastDescription);
                        ++s_PrivateInstances;
                        continue;

                    case "Image":
                        s_PrivateSubParams.Add(s_PropertyKey, CurrentBroadcastPicture);
                        ++s_PrivateInstances;
                        continue;

                    case "Tag":
                        s_PrivateSubParams.Add(s_PropertyKey, CurrentBroadcastCategoryTag);
                        ++s_PrivateInstances;
                        continue;

                    case "Privacy":
                        // TODO: Different privacy settings
                        s_PrivateSubParams.Add(s_PropertyKey, 0);
                        ++s_PrivateInstances;
                        continue;

                    case "totalListens":
                        // TODO: Total listens
                        s_PrivateSubParams.Add(s_PropertyKey, 0);
                        ++s_PrivateInstances;
                        continue;

                    case "isPlaying":
                        s_PrivateSubParams.Add(s_PropertyKey, IsPlaying);
                        ++s_PrivateInstances;
                        continue;

                    case "pendingNewOwnerUserDetails":
                        // TODO: Implement
                        s_PrivateSubParams.Add(s_PropertyKey, null);
                        ++s_PrivateInstances;
                        continue;

                    case "mobileCompliant":
                        // TODO: allow configuring this?
                        s_PrivateSubParams.Add(s_PropertyKey, 1);
                        ++s_PrivateInstances;
                        continue;

                    case "owners":
                    case "ownerUserIDs":
                    case "ownerArtistIDs":
                        s_PublicSubParams.Add(m_ChatChannelKeyMap["owners"] as String, GetChatVariableOwners());
                        ++s_PublicInstances;

                        s_PrivateSubParams.Add(m_ChatChannelKeyMap["owners"] as String,  GetChatVariableOwners());
                        ++s_PrivateInstances;
                        continue;

                    case "broadcasterVersion":
                        s_PublicSubParams.Add(s_PropertyKey, c_BroadcasterVersion);
                        ++s_PublicInstances;

                        s_PrivateSubParams.Add(s_PropertyKey, c_BroadcasterVersion);
                        ++s_PrivateInstances;
                        continue;

                    case "ownerSubscribed":
                        // TODO: Should this be configurable?
                        s_PublicSubParams.Add(s_PropertyKey, true);
                        ++s_PublicInstances;

                        s_PrivateSubParams.Add(s_PropertyKey, true);
                        ++s_PrivateInstances;
                        continue;
                }
            }

            //

            var s_Blackbox = new Dictionary<String, Object>()
            {
                { "source", p_Source },
                { "changedProps", p_ChangedProps }
            };

            if (p_Token != null)
                s_Blackbox.Add("token", p_Token);

            if (s_PublicInstances > 0)
            {
                if (s_Blackbox.ContainsKey("channelType"))
                    s_Blackbox["channelType"] = "public";
                else
                    s_Blackbox.Add("channelType", "public");

                Library.Chat.SetSubscriptionParameters(Library.Chat.GetChatChannel(ActiveBroadcastID, true), s_PublicSubParams, s_Blackbox);
            }

            if (s_PrivateInstances > 0)
            {
                if (s_Blackbox.ContainsKey("channelType"))
                    s_Blackbox["channelType"] = "private";
                else
                    s_Blackbox.Add("channelType", "private");

                Library.Chat.SetSubscriptionParameters(Library.Chat.GetChatChannel(ActiveBroadcastID), s_PrivateSubParams, s_Blackbox);
            }
        }

        private void ProcessUpdates(Dictionary<String, Object> p_ChangedProps, bool p_Unknown = true, String p_Source = null)
        {
            if (p_Unknown)
                UpdateChatServer(p_ChangedProps, p_Source);

            //DispatchUpdateEvents(p_ChangedProps);

            if (p_ChangedProps == null || p_ChangedProps.ContainsKey("Name") ||
                p_ChangedProps.ContainsKey("Description") || p_ChangedProps.ContainsKey("Image"))
                UpdateChatClientWithBroadcast();

            if (p_ChangedProps == null || p_ChangedProps.ContainsKey("suggestions"))
            {
                if (m_SuggestionChanges.Count < c_MaxSuggestionChanges) 
                    return;

                m_SuggestionChanges.Clear();
                ProcessUpdates(new Dictionary<string, object>() {{ "suggestions", true }});
            }
        }

        private void UpdateChatClientWithBroadcast()
        {
            // TODO: Properly set this.
            CurrentBroadcastStatus = String.IsNullOrWhiteSpace(ActiveBroadcastID)
                ? BroadcastStatus.Idle
                : BroadcastStatus.Broadcasting;

            Library.Chat.ReEvaluateMastershipAndUpdate();
        }
    }
}
