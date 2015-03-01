using System;

namespace GS.Lib.Models
{
    internal class Subscription : SharkObject
    {
        public String Name { get; set; }
        public String Type { get; set; }

        public Subscription(String p_Name)
        {
            Type = "sub";
            Name = p_Name;
        }
    }
}
