using System;
using System.Collections.Generic;

namespace GS.Lib.Models
{
    internal class SubscriptionKeys : SharkObject
    {
        public String Sub { get; set; }
        public List<String> Keys { get; set; } 
    }
}
