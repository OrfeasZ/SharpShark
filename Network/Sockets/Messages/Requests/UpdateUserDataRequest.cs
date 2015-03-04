using System;
using System.Collections.Generic;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages.Generic;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    class UpdateUserDataRequest : SharkMessage
    {
        public KeyValParams<Dictionary<String, Object>> Params { get; set; }

        public UpdateUserDataRequest(Dictionary<String, Object> p_Data)
            : base("set")
        {
            Params = new KeyValParams<Dictionary<String, Object>>();
            Params.Keyvals.Add(new KeyValEntry<Dictionary<String, Object>>("i", p_Data));
        }
    }
}
