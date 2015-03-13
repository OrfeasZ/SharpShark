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

        public CategoryTag()
        {
            Index = 0;
            Name = "multi-genre";
        }

        public CategoryTag(String p_Index, String p_Name)
        {
            Index = Int32.Parse(p_Index);
            Name = p_Name;
        }

        public CategoryTag(int p_Index, String p_Name)
        {
            Index = p_Index;
            Name = p_Name;
        }
    }
}
