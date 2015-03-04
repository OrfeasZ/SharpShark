using System;

namespace GS.Lib.Models
{
    internal class UserStatus : SharkObject
    {
        public Object SongEx { get; set; }
        public Object Song { get; set; }
        public int Status { get; set; }
        public UInt64 Time { get; set; }

        public String Bcast { get; set; }
        public String BcastPic { get; set; }
        public String BcastName { get; set; }
        public int BcastOwner { get; set; }
        public Object BcastOwnerInfo { get; set; }
    }
}
