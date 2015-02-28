using System;
using GS.Lib.Models;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class IdentifyRequest : SharkMessage
    {
        internal class IdentifyBlackbox
        {
            public bool Retried { get; set; }    
        }

        internal class IdentifyParams
        {
            [JsonProperty("app_data")]
            public ChatUserData AppData { get; set; }

            [JsonProperty("app_sig")]
            public String AppSig { get; set; }

            [JsonProperty("sessionid")]
            public String SessionID { get; set; }

            [JsonProperty("uid")]
            public String UUID { get; set; }

            public bool Invisible { get; set; }

            [JsonProperty("userid")]
            public String UserID { get; set; }
        }

        public Object Params { get; set; }
        
        public Object Blackbox { get; set; }

        public IdentifyRequest(ChatUserData p_ChatData, String p_ChatSignature, String p_SessionID, Guid p_UUID, bool p_Invisible, Int64 p_UserID) 
            : base("identify")
        {
            Params = new IdentifyParams()
            {
                AppData = p_ChatData,
                AppSig = p_ChatSignature,
                SessionID = p_SessionID,
                UUID = p_UUID.ToString("D"),
                Invisible = p_Invisible,
                UserID = p_UserID.ToString()
            };

            Blackbox = new IdentifyBlackbox
            {
                Retried = false
            };
        }

    }
}
