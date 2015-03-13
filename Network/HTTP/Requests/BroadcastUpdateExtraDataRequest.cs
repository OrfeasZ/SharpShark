using System;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class BroadcastUpdateExtraDataRequest : SharkObject
    {
        public String BroadcastID { get; set; }
        public String Name { get; set; }
        public CategoryTag Tag { get; set; }
        public String Description { get; set; }
        public String Image { get; set; }
        public String Privacy { get; set; }
    }
}
