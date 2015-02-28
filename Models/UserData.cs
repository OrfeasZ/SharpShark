using System;

namespace GS.Lib.Models
{
    internal class UserData : SharkObject
    {
        public String Email { get; set; }
        public String FName { get; set; }
        public String LName { get; set; }
        public bool IsActive { get; set; }
        public String Picture { get; set; }
        public Int64 UserID { get; set; }
        public ChatUserData ChatUserData { get; set; }
        public String ChatUserDataSig { get; set; }
    }
}
