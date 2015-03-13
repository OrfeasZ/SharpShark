using System;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Models
{
    internal class KeyValData : SharkObject
    {
        public String Key { get; set; }

        public JToken Value { get; set; }
    }
}
