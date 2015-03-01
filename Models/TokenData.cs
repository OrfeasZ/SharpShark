using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    internal class TokenData : SharkObject
    {
        internal class GSConfig : SharkObject
        {
            public String SessionID { get; set; }
            public UserData User { get; set; }
            public CountryData Country { get; set; }
            public Dictionary<String, int> ChatServersWeighted { get; set; }

            [JsonProperty("uuid")]
            public Guid UUID { get; set; }
        }

        public String GetCommunicationToken { get; set; }
        public GSConfig GetGSConfig { get; set; }
    }
}
