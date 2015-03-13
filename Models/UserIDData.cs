using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    internal class UserIDData : SharkObject
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        public UserIDData(Int64 p_UserID)
        {
            Type = "userid";
            Name = p_UserID.ToString();
        }
    }
}
