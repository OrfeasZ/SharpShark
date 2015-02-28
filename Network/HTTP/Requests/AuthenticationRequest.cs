using System;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class AuthenticationRequest : SharkObject
    {
        public String Username { get; set; }
        public String Password { get; set; }
    }
}
