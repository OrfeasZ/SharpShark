using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    internal class PlaybackStatusData
    {
        internal class ActiveBroadcastData
        {
            internal class SongData
            {
                [JsonProperty("sID")]
                public Int64 SongID { get; set; }

                [JsonProperty("sN")]
                public String SongName { get; set; }

                [JsonProperty("arID")]
                public Int64 ArtistID { get; set; }

                [JsonProperty("arN")]
                public String ArtistName { get; set; }

                [JsonProperty("alID")]
                public Int64 AlbumID { get; set; }

                [JsonProperty("alN")]
                public String AlbumName { get; set; }
            }

            [JsonProperty("b")]
            public SongData Data { get; set; }

            public Int64 QueueSongID { get; set; }
        }

        public int Status { get; set; }

        [JsonProperty("ts")]
        public Int64 Timestamp { get; set; }

        [JsonProperty("pos")]
        public double Position { get; set; }

        public ActiveBroadcastData Active { get; set; }

        public String OwnerAvailable { get; set; }

        public bool Ended { get; set; }
    }
}
