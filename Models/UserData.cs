using System;

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

        internal ChatUserData ChatUserData { get; set; }

        internal String ChatUserDataSig { get; set; }

        public CategoryTag Tag { get; set; }
    }
}
