using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Generic;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    class UpdateUserDataRequest : SharkMessage
    {
        public KeyValParams<ChatUserData> Params { get; set; }

        public UpdateUserDataRequest(ChatUserData p_Data)
            : base("set")
        {
            Params = new KeyValParams<ChatUserData>();
            Params.Keyvals.Add(new KeyValEntry<ChatUserData>("i", p_Data));
        }
    }
}
