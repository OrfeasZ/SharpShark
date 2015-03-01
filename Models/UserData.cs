using System;
using Newtonsoft.Json;

namespace GS.Lib.Models
{
    public class UserData : SharkObject
    {
        public String Email { get; set; }

        public String FName { get; set; }

        public String LName { get; set; }

        public bool IsActive { get; set; }

        public String Picture { get; set; }

        public Int64 UserID { get; set; }

        public Int64 ArtistID { get; set; }

        [JsonProperty("chatUserData")]
        internal ChatUserData ChatUserData { get; set; }

        [JsonProperty("chatUserDataSig")]
        internal String ChatUserDataSig { get; set; }

        public CategoryTag Tag { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String Country { get; set; }
    }
}
