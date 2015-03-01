using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    internal class MetaUserData : SharkObject
    {
        internal class MetaUserIDData
        {
            public bool Sudo { get; set; }

            [JsonProperty("uid")]
            public String UID { get; set; }

            [JsonProperty("userid")]
            public String UserID { get; set; }

            [JsonProperty("app_data")]
            public ChatUserData AppData { get; set; }
        }

        [JsonProperty("userid")]
        public String UserID { get; set; }

        public String Status { get; set; }

        [JsonProperty("latest_id")]
        public MetaUserIDData LatestID { get; set; }
    }
}
