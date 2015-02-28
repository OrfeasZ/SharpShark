using System;
using GS.Lib.Models;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    class IdentifyResponse : SharkMessage
    {
        internal class ChatIDContainer
        {
            public bool Sudo { get; set; }
            
            [JsonProperty("uid")]
            public String UID { get; set; }

            [JsonProperty("userid")]
            public String UserID { get; set; }

            [JsonProperty("app_data")]
            public ChatUserData AppData { get; set; }
        }

        internal class IdentifySuccess
        {
            [JsonProperty("id")]
            public ChatIDContainer ID { get; set; }

            [JsonProperty("loggedin")]
            public bool LoggedIn { get; set; }
        }

        internal class IdentifyBlackbox
        {
            public bool Retried { get; set; }
        }

        public IdentifySuccess Success { get; set; }

        public IdentifyBlackbox Blackbox { get; set; }

        public IdentifyResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
