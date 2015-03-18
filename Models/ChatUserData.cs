using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    public class ChatUserData : SharkObject
    {
        [JsonProperty("p")]
        public String Picture { get; set; }

        [JsonProperty("z")]
        public String ZToken { get; set; }

        [JsonProperty("y")]
        internal String Unknown01 { get; set; }

        [JsonProperty("n")]
        public String Username { get; set; }

        [JsonProperty("f")]
        internal String Unknown02 { get; set; }
    }
}
