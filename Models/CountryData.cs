using System;

namespace GS.Lib.Models
{
    public class CountryData : SharkObject
    {
        public Int64 ID { get; set; }
        public Int64 CC1 { get; set; }
        public Int64 CC2 { get; set; }
        public Int64 CC3 { get; set; }
        public Int64 CC4 { get; set; }
        public Int64 GMA { get; set; }
        public String Iso { get; set; }
        public String Region { get; set; }
        public String City { get; set; }
        public String Zip { get; set; }
        public Int64 IPR { get; set; }

        public CountryData()
        {
            ID = 85;
            CC2 = 1048576;
            Iso = "GR";
            Region = "35";
            City = "Athens";
            Zip = "";
        }
    }
}
