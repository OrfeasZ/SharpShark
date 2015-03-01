using System;

namespace GS.Lib.Models
{
    public class FollowerFollowingUserData : SharkObject
    {
        public String UserID { get; set; }

        public String ArtistID { get; set; }

        public String FName { get; set; }

        public String Picture { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String Country { get; set; }

        internal ChatUserData ChatUserData { get; set; }

        internal String ChatUserDataSig { get; set; }

        public CategoryTag Tag { get; set; }

        public String IsPremium { get; set; }

        public String IsArtist { get; set; }

        public String IsFollower { get; set; }

        public String IsFavorite { get; set; }

        public String TSFavorited { get; set; }

        public String FollowingFlags { get; set; }
    }
}
