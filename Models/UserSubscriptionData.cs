using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Lib.Models
{
    internal class UserSubscriptionData : SharkObject
    {
        public List<String> Keys { get; set; }

        public String UserID { get; set; }

        public UserSubscriptionData(Int64 p_UserID, IEnumerable<String> p_SubscriptionKeys = null)
        {
            Keys = p_SubscriptionKeys == null ? new List<string>() : p_SubscriptionKeys.ToList();
            UserID = p_UserID.ToString();
        }

        public UserSubscriptionData(String p_UserID, IEnumerable<String> p_SubscriptionKeys = null)
        {
            Keys = p_SubscriptionKeys == null ? new List<string>() : p_SubscriptionKeys.ToList();
            UserID = p_UserID;
        }
    }
}
