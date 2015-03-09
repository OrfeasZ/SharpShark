using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public RequestParameters Params { get; set; }

        public SubscribeToMetaArtistsRequest(List<String> p_ArtistIDs, int p_Parts, int p_PartID, bool p_Initial, bool p_Retried)
            : base("meta_sub")
        {
            Params = new RequestParameters()
            {
                Type = "artistids",
                ArtistIDs = p_ArtistIDs
            };

            Blackbox = new Dictionary<string, JToken>()
            {
                { "source", "subscribeToMetaArtists" },
                { "partsTotal", p_Parts },
                { "partID", p_PartID },
                { "initial", p_Initial },
                { "retried", p_Retried }
            };
        }
    }
}
