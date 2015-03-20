using System;
using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Requests;
using GS.Lib.Network.Sockets.Messages.Responses;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        internal void GetSubscriberList(String p_Channel, Action<List<SimpleUserData>> p_Callback)
        {
            if (Library.User.Data == null)
            {
                p_Callback(new List<SimpleUserData>());
                return;
            }

            m_SocketClient.SendMessage(new GetSubscriberListRequest(p_Channel, true), p_Message =>
            {
                if (p_Message == null)
                {
                    p_Callback(new List<SimpleUserData>());
                    return;
                }

                var s_Message = p_Message.As<GetSubscriberListResponse>();

                if (s_Message == null)
                {
                    p_Callback(new List<SimpleUserData>());
                    return;
                }

                var s_Users = new List<SimpleUserData>();

                foreach (var s_User in s_Message.SubList)
                {
                    var s_Data = s_User.ToObject<Dictionary<String, JToken>>();
                    
                    JToken s_IDToken;
                    if (!s_Data.TryGetValue("id", out s_IDToken))
                        continue;

                    var s_ID = s_IDToken.ToObject<Dictionary<String, JToken>>();

                    if (!s_ID.ContainsKey("app_data") || !s_ID.ContainsKey("userid"))
                        continue;

                    var s_UserData = s_ID["app_data"].ToObject<ChatUserData>();
                    var s_UserID = Int64.Parse(s_ID["userid"].Value<String>());

                    s_Users.Add(new SimpleUserData()
                    {
                        UserID = s_UserID,
                        UserData = s_UserData
                    });
                }

                p_Callback(s_Users);
            });
        }

        internal void GetSubscriberCount(String p_Channel, Action<Int64> p_Callback)
        {
            if (Library.User.Data == null)
            {
                p_Callback(0);
                return;
            }

            m_SocketClient.SendMessage(new SubCountRequest(p_Channel), p_Message =>
            {
                if (p_Message == null)
                {
                    p_Callback(0);
                    return;
                }

                var s_Message = p_Message.As<SubCountResponse>();

                if (s_Message == null)
                {
                    p_Callback(0);
                    return;
                }

                p_Callback(s_Message.SubCount);
            });
        }


    }
}
