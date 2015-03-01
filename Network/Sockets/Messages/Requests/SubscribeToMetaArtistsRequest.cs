using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class SubscribeToMetaArtistsRequest : SharkMessage
    {
        internal class RequestParameters
        {
            public String Type { get; set; }

            [JsonProperty("artistids")]
            public List<String> ArtistIDs { get; set; }
        }

        internal class RequestBlackbox
        {
            public String Source { get; set; }

            public int PartsTotal { get; set; }

            public int PartID { get; set; }

            public bool Initial { get; set; }

            public bool Retried { get; set; }
        }

        public RequestParameters Params { get; set; }

        public RequestBlackbox Blackbox { get; set; }

        public SubscribeToMetaArtistsRequest(List<String> p_ArtistIDs, int p_Parts, int p_PartID, bool p_Initial, bool p_Retried)
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "artistids",
                ArtistIDs = p_ArtistIDs
            };

            Blackbox = new RequestBlackbox()
            {
                Source = "subscribeToMetaArtists",
                PartsTotal = p_Parts,
                PartID = p_PartID,
                Initial = p_Initial,
                Retried = p_Retried
            };
        }
    }
}
