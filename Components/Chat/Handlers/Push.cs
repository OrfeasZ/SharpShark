using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandlePush(SharkResponseMessage p_Message)
        {
            var s_BasicResponse = p_Message.As<BasicResponse>();

            if (s_BasicResponse.Type == "logged_in" && HandlePushLoggedIn(p_Message))
                return;

            if (s_BasicResponse.Type == "logged_out" && HandlePushLoggedOut(p_Message))
                return;

            if (s_BasicResponse.Type == "key_change" && HandlePushKeyChange(p_Message))
                return;

            if (s_BasicResponse.Type == "subinfo_change" && HandlePushSubinfoChange(p_Message))
                return;

            if (s_BasicResponse.Type == "publish" && HandlePushPublish(p_Message))
                return;

            if (s_BasicResponse.Type == "sub_alert" && HandlePushSubAlert(p_Message))
                return;

            if (s_BasicResponse.Type == "unsub_alert" && HandlePushUnsubAlert(p_Message))
                return;

            if (s_BasicResponse.Type == "sub_rename" && HandlePushSubRename(p_Message))
                return;
        }

        private bool HandlePushLoggedIn(SharkResponseMessage p_Message)
        {
            // TODO: Implement.
            return true;
        }

        private bool HandlePushLoggedOut(SharkResponseMessage p_Message)
        {
            // TODO: Implement.
            return true;
        }

        private bool HandlePushKeyChange(SharkResponseMessage p_Message)
        {
            var s_Message = p_Message.As<KeyChangeResponse>();

            if (s_Message.KeyChange == null || s_Message.KeyChange.KeyVals == null)
                return false;

            if (!String.IsNullOrWhiteSpace(s_Message.KeyChange.UserID))
            {
                // TODO: Implement.
                return true;
            }

            if (!String.IsNullOrWhiteSpace(s_Message.KeyChange.ArtistID))
            {
                // TODO: Implement.
                return true;
            }

            if (!String.IsNullOrWhiteSpace(s_Message.KeyChange.Sub))
            {
                var s_MetaSubChanges = s_Message.KeyChange.KeyVals.Where(p_KeyVal => p_KeyVal != null)
                        .ToDictionary(p_KeyVal => p_KeyVal.Key, p_KeyVal => p_KeyVal.Value);

                DispatchEvent((int) ChatEvent.SubUpdate, new SubUpdateEvent()
                {
                    Type = "properties",
                    Sub = s_Message.KeyChange.Sub,
                    Params = s_MetaSubChanges
                });

                return true;
            }

            return true;
        }

        private bool HandlePushSubinfoChange(SharkResponseMessage p_Message)
        {
            var s_Message = p_Message.As<SubinfoChangeResponse>();

            if (s_Message.SubinfoChange == null)
                return false;

            DispatchEvent((int)ChatEvent.SubUpdate, new SubUpdateEvent()
            {
                Type = "properties",
                Sub = s_Message.SubinfoChange.Sub,
                Params = s_Message.SubinfoChange.Params
            });

            return true;
        }

        private bool HandlePushPublish(SharkResponseMessage p_Message)
        {
            var s_Message = p_Message.As<PublishResponse>();

            if (s_Message.Publish == null)
                return false;

            if (Library.User.Data != null && Library.User.Data.ArtistID > 0 &&
                s_Message.Publish.Destination == "artist:" + Library.User.Data.ArtistID && s_Message.Publish.ID != null &&
                ((s_Message.Publish.ID.ContainsKey("artistid") && s_Message.Publish.ID["artistid"].Value<String>() != "False" &&
                  Int64.Parse(s_Message.Publish.ID["artistid"].Value<String>() ?? "0") == Library.User.Data.ArtistID) ||
                 (s_Message.Publish.ID.ContainsKey("sudo") && (bool) s_Message.Publish.ID["sudo"])))
                HandleSelfMessages(s_Message);
            else if (Library.User.Data != null && Library.User.Data.UserID > 0 &&
                s_Message.Publish.Destination == "user:" + Library.User.Data.UserID && s_Message.Publish.ID != null &&
                ((s_Message.Publish.ID.ContainsKey("userid") && s_Message.Publish.ID["userid"].Value<String>() != "False" &&
                  Int64.Parse(s_Message.Publish.ID["userid"].Value<String>() ?? "0") == Library.User.Data.UserID) ||
                 (s_Message.Publish.ID.ContainsKey("sudo") && (bool)s_Message.Publish.ID["sudo"])))
                HandleSelfMessages(s_Message);

            // Is this a chat message?
            var s_Value = s_Message.Publish.Value;

            if (s_Value.ContainsKey("type") && s_Value["type"].Value<String>() == "chat")
            {
                var s_ID = s_Message.Publish.ID ?? new Dictionary<string, JToken>();

                var s_ChatMessage = s_Value["data"].Value<String>();

                var s_UserID = s_ID.ContainsKey("userid") ? Int64.Parse(s_ID["userid"].Value<String>() ?? "0") : 0;
                var s_UserAppData = s_ID.ContainsKey("app_data") ? s_ID["app_data"] : null;

                String s_UserName = null;

                if (s_UserAppData != null)
                    s_UserName = s_UserAppData["n"].Value<String>();

                Library.DispatchEvent(ClientEvent.ChatMessage, new ChatMessageEvent()
                {
                    DestinationChannel = s_Message.Publish.Destination,
                    UserID = s_UserID,
                    UserName = s_UserName,
                    ChatMessage = s_ChatMessage
                });
            }

            DispatchEvent((int)ChatEvent.SubUpdate, new SubUpdateEvent()
            {
                Type = "publish",
                Sub = s_Message.Publish.Destination,
                ID = s_Message.Publish.ID,
                Value = s_Message.Publish.Value
            });

            return true;
        }

        private bool HandlePushSubAlert(SharkResponseMessage p_Message)
        {
            // TODO: Implement.
            return true;
        }

        private bool HandlePushUnsubAlert(SharkResponseMessage p_Message)
        {
            // TODO: Implement.
            return true;
        }

        private bool HandlePushSubRename(SharkResponseMessage p_Message)
        {
            var s_Message = p_Message.As<SubRenameResponse>();

            if (m_CurrentChannels.ContainsKey(s_Message.SubRename.OldName))
            {
                if (m_CurrentChannels.ContainsKey(s_Message.SubRename.NewName))
                    m_CurrentChannels[s_Message.SubRename.NewName] = m_CurrentChannels[s_Message.SubRename.OldName];
                else
                    m_CurrentChannels.Add(s_Message.SubRename.NewName, m_CurrentChannels[s_Message.SubRename.OldName]);

                if (m_CurrentChannels[s_Message.SubRename.NewName].ContainsKey("sub"))
                    m_CurrentChannels[s_Message.SubRename.NewName]["sub"] = s_Message.SubRename.NewName;
                else
                    m_CurrentChannels[s_Message.SubRename.NewName].Add("sub", s_Message.SubRename.NewName);

                m_CurrentChannels.Remove(s_Message.SubRename.OldName);
            }

            DispatchEvent((int)ChatEvent.SubUpdate, new SubUpdateEvent()
            {
                Type = "rename",
                Sub = s_Message.SubRename.OldName,
                NewName = s_Message.SubRename.NewName
            });

            return true;
        }
    }
}
