using System;

namespace GS.Lib.Models
{
    public class CountryData : SharkObject
    {
        public int ID { get; set; }
        public int CC1 { get; set; }
        public int CC2 { get; set; }
        public int CC3 { get; set; }
        public int CC4 { get; set; }
        public int GMA { get; set; }
        public String Iso { get; set; }
        public String Region { get; set; }
        public String City { get; set; }
        public String Zip { get; set; }
        public int IPR { get; set; }

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
