using System;
using GS.Lib.Models;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class CreateBroadcastRequest : SharkObject
    {
        public String Name { get; set; }

        public bool IsRandomName { get; set; }

        public String Description { get; set; }

        public CategoryTag Tag { get; set; }
    }
}
