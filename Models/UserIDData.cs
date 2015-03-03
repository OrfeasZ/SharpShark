using System;

namespace GS.Lib.Models
{
    internal class UserIDData : SharkObject
    {
        public String Type { get; set; }

        public String Name { get; set; }

        public UserIDData(Int64 p_UserID)
        {
            Type = "userid";
            Name = p_UserID.ToString();
        }
    }
}
