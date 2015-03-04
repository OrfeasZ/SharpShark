using System;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SubRenameResponse : SharkMessage
    {
        internal class SubRenameData
        {
            [JsonProperty("old_name")]
            public String OldName { get; set; }

            [JsonProperty("new_name")]
            public String NewName { get; set; }
        }

        [JsonProperty("sub_rename")]
        public SubRenameData SubRename { get; set; }

        public SubRenameResponse(string p_Command) 
            : base(p_Command)
        {
        }
    }
}
