﻿using System;
using System.Collections.Generic;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class GetSubscriberListRequest : SharkMessage
    {
        public Dictionary<String, String> Params { get; set; }

        public Dictionary<String, Object> Blackbox { get; set; } 

        public GetSubscriberListRequest(String p_SubList, bool p_WithExtra, Dictionary<String, Object> p_Blackbox) 
            : base("info")
        {
            if (p_WithExtra)
            {
                Params = new Dictionary<string, string>()
                {
                    {"type", "sub_list_with_extra"},
                    {"sub_list_with_extra", p_SubList}
                };
            }
            else
            {
                Params = new Dictionary<string, string>()
                {
                    {"type", "sub_list"},
                    {"sub_list", p_SubList}
                };
            }

            Blackbox = p_Blackbox;
        }
    }
}