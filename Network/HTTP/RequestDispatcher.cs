using System;
using System.Net;
using GS.Lib.Network.HTTP.Requests;
using GS.Lib.Network.HTTP.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GS.Lib.Network.HTTP
{
    internal class RequestDispatcher
    {
        public SharpShark Library { get; private set; }

        private readonly String m_SecretKey01;
        private readonly String m_SecretKey02;

        public RequestDispatcher(SharpShark p_Library, String p_SecretKey01, String p_SecretKey02)
        {
            Library = p_Library;

            m_SecretKey01 = p_SecretKey01;
            m_SecretKey02 = p_SecretKey02;
        }

        public Z Dispatch<T, Z>(String p_Method, T p_Parameters)
            where Z : SharkObject
            where T : SharkObject
        {
            return DispatchInternal<T, Z>(p_Method, p_Parameters, NeedsHTTPS(p_Method));
        }

        private Z DispatchInternal<T, Z>(String p_Method, T p_Parameters, bool p_UseHTTPS)
            where Z : SharkObject
            where T : SharkObject
        {
            // Check if we're authenticated.
            if (String.IsNullOrWhiteSpace(Library.Authenticator.SessionID) ||
                Library.Authenticator.UUID == Guid.Empty)
                return null;

            using (var s_Client = new WebClient())
            {
                s_Client.Headers.Add(HttpRequestHeader.Cookie, "PHPSESSID=" + Library.Authenticator.SessionID);
                s_Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                s_Client.Headers.Add(HttpRequestHeader.Accept, "application/json");

                // Determine the SecretKey we need to use.
                var s_SecretKey = DetermineSecretKey(p_Method);

                // Construct our request.
                var s_Request = new SharkRequest<T>(p_Method, Library.Authenticator.SessionID,
                    Library.Authenticator.UUID, Library.Authenticator.CommunicationToken,
                    s_SecretKey, p_Parameters);

                string s_ResponseData;

                try
                {
                    // Send our request and download the response.
                    var s_SerializedRequest = JsonConvert.SerializeObject(s_Request, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    var s_URL = Library.BaseURL + "/more.php?" + p_Method;

                    if (p_UseHTTPS)
                        s_URL = s_URL.Replace("http://", "https://");

                    s_ResponseData = s_Client.UploadString(s_URL + p_Method, s_SerializedRequest);
                }
                catch
                {
                    return null;
                }

                // If the request requires HTTPS silently retry it with HTTPS.
                if (s_ResponseData == "HTTPS Required" && !p_UseHTTPS)
                    return DispatchInternal<T, Z>(p_Method, p_Parameters, true);

                try
                {
                    // Try to parse the response.
                    var s_Response = JsonConvert.DeserializeObject<SharkResponse<Z>>(s_ResponseData);

                    // TODO: Additional checks?

                    return s_Response.Result;
                }
                catch
                {
                    return null;
                }
            }
        }

        private String DetermineSecretKey(String p_Method)
        {
            switch (p_Method)
            {
                case "getStreamKeyFromSongIDEx":
                case "markSongDownloadedEx":
                case "markSongQueueSongPlayed":
                case "markStreamKeyOver30Seconds":
                case "getQueueSongListFromSongIDs":
                case "removeSongsFromQueue":
                case "addSongsToQueue":
                case "getTokens":
                    return m_SecretKey01;
                    
                default:
                    return m_SecretKey02;
            }
        }

        private bool NeedsHTTPS(String p_Method)
        {
            switch (p_Method)
            {
                case "authenticateUser":
                    return true;

                default:
                    return false;
            }
        }

    }
}
