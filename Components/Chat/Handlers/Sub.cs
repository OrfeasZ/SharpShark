using System;
using System.Collections.Generic;
using System.Diagnostics;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void HandleSub(SharkResponseMessage p_Message)
        {
            switch (p_Message.As<BasicResponse>().Type)
            {
                case "error":
                    {
                        Debug.WriteLine(String.Format("Sub failed. Error: {0}", p_Message.As<ErrorResponse<Object>>().Error));
                        break;
                    }

                case "success":
                    {
                        var s_Message = p_Message.As<SubscriptionResponse>();
                        HandleSub(s_Message.Success, s_Message.Blackbox);
                        break;
                    }

                default:
                    {
                        Debug.WriteLine("Unknown sub response type received!");
                        break;
                    }
            }
        }

        private void HandleSub(List<Dictionary<String, Object>> p_Channels, Dictionary<String, JToken> p_Blackbox = null)
        {
            if (m_CurrentChannels == null)
                m_CurrentChannels = new Dictionary<string, Dictionary<string, object>>();

            var s_NewChannels = new Dictionary<String, Dictionary<String, Object>>();
            var s_ChannelParams = new Dictionary<String, Object>();

            foreach (var s_Channel in p_Channels)
            {
                if (!s_Channel.ContainsKey("sub"))
                    continue;

                Object s_Sub;
                if (!s_Channel.TryGetValue("sub", out s_Sub))
                    continue;

                if (String.IsNullOrWhiteSpace(s_Sub as String))
                    continue;

                Object s_Error;
                if (s_Channel.TryGetValue("error", out s_Error))
                {
                    // TODO: Handle errors.
                    continue;
                }

                if (s_Channel.ContainsKey("params"))
                {
                    s_ChannelParams.Add(s_Sub as String, s_Channel["params"]);
                    s_Channel.Remove("params");
                }

                // TODO: Handle latest_messages

                if (s_Channel.ContainsKey("overwrite_params"))
                    s_Channel["overwrite_params"] = false;
                else
                    s_Channel.Add("overwrite_params", false);

                if (m_CurrentChannels.ContainsKey(s_Sub as String) &&
                    (
                        (m_CurrentChannels[s_Sub as String].ContainsKey("create_when_dne") &&
                         !(bool) m_CurrentChannels[s_Sub as String]["create_when_dne"]) ||
                        (m_CurrentChannels[s_Sub as String].ContainsKey("try_to_own") &&
                         !(bool) m_CurrentChannels[s_Sub as String]["try_to_own"])
                        ))
                {
                    if (s_Channel.ContainsKey("create_when_dne"))
                        s_Channel["create_when_dne"] = false;
                    else
                        s_Channel.Add("create_when_dne", false);

                    if (s_Channel.ContainsKey("try_to_own"))
                        s_Channel["try_to_own"] = false;
                    else
                        s_Channel.Add("try_to_own", false);
                }

                s_NewChannels.Add(s_Sub as String, s_Channel);
            }

            // Update channel list
            m_CurrentChannels = s_NewChannels;
            m_ChannelListDirty = false;

            if (Library.User.Data != null && Library.User.Data.ArtistID != 0 &&
                m_CurrentChannels.ContainsKey("artist:" + Library.User.Data.ArtistID))
            {
                // TODO: Fix; this condition is incomplete.
                if (!s_ChannelParams.ContainsKey("artist:" + Library.User.Data.ArtistID))
                {
                    SetSubscriptionParameters("artist:" + Library.User.Data.ArtistID, new Dictionary<string, object>()
                    {
                        {"unsub_alert", false},
                        {"sub_alert", true},
                        {
                            "owners", new List<Dictionary<String, String>>()
                            {
                                new Dictionary<string, string>()
                                {
                                    {"type", "userid"},
                                    {"name", Library.User.Data.UserID.ToString()}
                                },
                                new Dictionary<string, string>()
                                {
                                    {"type", "artistid"},
                                    {"name", Library.User.Data.ArtistID.ToString()}
                                },
                            }
                        }
                    });
                }

                if (LoggedInMaster == null)
                    PingMaster();
            }
            else if (Library.User.Data != null && Library.User.Data.UserID != 0 &&
                     m_CurrentChannels.ContainsKey("user:" + Library.User.Data.UserID))
            {
                // TODO: Fix; this condition is incomplete.
                if (!s_ChannelParams.ContainsKey("user:" + Library.User.Data.UserID))
                {
                    SetSubscriptionParameters("user:" + Library.User.Data.UserID, new Dictionary<string, object>()
                    {
                        {"unsub_alert", true},
                        {"sub_alert", true},
                        {
                            "owners", new List<Dictionary<String, String>>()
                            {
                                new Dictionary<string, string>()
                                {
                                    {"type", "userid"},
                                    {"name", Library.User.Data.UserID.ToString()}
                                }
                            }
                        }
                    });
                }

                if (LoggedInMaster == null)
                    PingMaster();
            }

            DispatchEvent((int)ChatEvent.SubResult, new SubResultEvent()
            {
                ChannelParameters = s_ChannelParams,
                Channels = s_NewChannels,
                Blackbox = p_Blackbox ?? new Dictionary<string, JToken>()
            });
        }
    }
}
