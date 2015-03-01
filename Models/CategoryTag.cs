using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    public class CategoryTag : SharkObject
    {
        [JsonProperty("i")]
        public int Index { get; set; }

        [JsonProperty("n")]
        public String Name { get; set; }
    }
}
