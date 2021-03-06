﻿using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    public class StatusSongData : SharkObject
    {
        [JsonProperty("CoverArtFilename")]
        public String CoverArtFilename { get; set; }

        [JsonProperty("TrackNum")]
        public Int64? TrackNum { get; set; }

        [JsonProperty("EstimateDuration")]
        public Int64? EstimateDuration { get; set; }

        [JsonProperty("AlbumID")]
        public Int64 AlbumID { get; set; }

        [JsonProperty("AlbumName")]
        public String AlbumName { get; set; }

        [JsonProperty("ArtistName")]
        public String ArtistName { get; set; }

        [JsonProperty("SongID")]
        public Int64 SongID { get; set; }

        [JsonProperty("SongName")]
        public String SongName { get; set; }

        [JsonProperty("Name")]
        public String Name { get; set; }

        [JsonProperty("Flags")]
        public Int64? Flags { get; set; }

        [JsonProperty("ArtistID")]
        public Int64 ArtistID { get; set; }
    }
}
