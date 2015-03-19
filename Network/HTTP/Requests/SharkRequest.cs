using System;
using System.Security.Cryptography;
using System.Text;
using GS.Lib.Models;
using GS.Lib.Util;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class SharkRequest<T>
    {
        internal class RequestHeader
        {
            public String Client { get; set; }
            public String ClientRevision { get; set; }
            public Int64 Privacy { get; set; }
            public CountryData Country { get; set; }
            public String Session { get; set; }
            public String Token { get; set; }

            public RequestHeader(String p_Method, String p_SessionID, String p_CommunicationToken, String p_SecretKey, CountryData p_CountryData = null)
            {
                Client = "htmlshark";
                ClientRevision = "20130520";
                Privacy = 0;
                Country = p_CountryData ?? new CountryData();
                Session = p_SessionID;

                var s_Randomizer = "abcdefghijklmnopqrstuvwxyz0123456789".SecureRandom(6);

                var s_HashToken = p_Method + ":" + p_CommunicationToken + ":" + p_SecretKey + ":" + s_Randomizer;

                using (var s_Hash = new SHA1Managed())
                    s_HashToken =
                        BitConverter.ToString(s_Hash.ComputeHash(Encoding.ASCII.GetBytes(s_HashToken)))
                            .Replace("-", "")
                            .ToLower();

                Token = s_Randomizer + s_HashToken;
            }
        }

        public RequestHeader Header { get; set; }
        public String Method { get; set; }
        public T Parameters { get; set; }

        public SharkRequest(String p_Method, String p_SessionID, String p_CommunicationToken, String p_SecretKey, T p_Parameters, CountryData p_CountryData = null)
        {
            Header = new RequestHeader(p_Method, p_SessionID, p_CommunicationToken, p_SecretKey, p_CountryData);
            Method = p_Method;
            Parameters = p_Parameters;
        }
    }
}
