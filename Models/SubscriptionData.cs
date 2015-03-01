using System;

namespace GS.Lib.Models
{
    internal class SubscriptionData : SharkObject
    {
        public String Name { get; set; }
        public String Type { get; set; }

        public SubscriptionData(String p_Name)
        {
            Type = "sub";
            Name = p_Name;
        }
    }
}
