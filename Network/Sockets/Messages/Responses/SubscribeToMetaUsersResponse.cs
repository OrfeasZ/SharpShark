using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Generic;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SubscribeToMetaUsersResponse : SharkMessage
    {
        public List<MetaUserData> Success { get; set; }

        public PartitionedBlackbox Blackbox { get; set; }

        public SubscribeToMetaUsersResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
