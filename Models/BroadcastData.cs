using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Lib.Models
{
    public class BroadcastData : SharkObject
    {
        public String BroadcastID { get; set; }

        public Int64 UserID { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public UInt32 TSModified { get; set; }

        public UInt32 TSCreated { get; set; }

        public int Privacy { get; set; }

        public int IsRandomName { get; set; }

        public int PlayCount { get; set; }
    }
}
