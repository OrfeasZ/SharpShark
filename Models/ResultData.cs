using System;

namespace GS.Lib.Models
{
    public class ResultData : SharkObject
    {
        public String AlbumID { get; set; }

        public String ArtistID { get; set; }

        public String SongID { get; set; }

        public String SongName { get; set; }

        public String AlbumName { get; set; }

        public String ArtistName { get; set; }

        public String Year { get; set; }

        public String TrackNum { get; set; }

        public String CoverArtFilename { get; set; }

        public String TSAdded { get; set; }

        public String AvgDuration { get; set; }

        public String EstimateDuration { get; set; }

        public int Flags { get; set; }

        public String IsLowBitrateAvailable { get; set; }

        public String IsVerified { get; set; }

        public int Popularity { get; set; }

        public Double Score { get; set; }

        public int RawScore { get; set; }

        public int PopularityIndex { get; set; }

        public String Name { get; set; }

        public String UserID { get; set; }
    }
}
