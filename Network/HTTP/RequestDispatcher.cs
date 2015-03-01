using System;
using System.Collections.Generic;
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
        {
            return DispatchInternal<T, Z>(p_Method, p_Parameters, NeedsHTTPS(p_Method));
        }

        private Z DispatchInternal<T, Z>(String p_Method, T p_Parameters, bool p_UseHTTPS)
        {
            // Check if we're authenticated.
            if (String.IsNullOrWhiteSpace(Library.User.SessionID) ||
                Library.User.UUID == Guid.Empty)
                return default(Z);

            using (var s_Client = new WebClient())
            {
                s_Client.Headers.Add(HttpRequestHeader.Cookie, "PHPSESSID=" + Library.User.SessionID);
                s_Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                s_Client.Headers.Add(HttpRequestHeader.Accept, "application/json");

                // Determine the SecretKey we need to use.
                var s_SecretKey = DetermineSecretKey(p_Method);

                if (p_Parameters == null)
                {
                    // Construct our request.
                    var s_EmptyRequest = new SharkRequest<Dictionary<String, String>>(p_Method, Library.User.SessionID,
                        Library.User.UUID, Library.User.CommunicationToken,
                        s_SecretKey, new Dictionary<string, string>(), Library.User.CountryData);

                    return RequestInternal<Dictionary<String, String>, Z>(s_Client, p_Method, new Dictionary<string, string>(), p_UseHTTPS, s_EmptyRequest);
                }

                // Construct our request.
                var s_Request = new SharkRequest<T>(p_Method, Library.User.SessionID,
                    Library.User.UUID, Library.User.CommunicationToken,
                    s_SecretKey, p_Parameters, Library.User.CountryData);

                return RequestInternal<T, Z>(s_Client, p_Method, p_Parameters, p_UseHTTPS, s_Request);
            }
        }

        private Z RequestInternal<T, Z>(WebClient p_Client, String p_Method, T p_Parameters, bool p_UseHTTPS, SharkRequest<T> p_Request)
        {
            string s_ResponseData;

            try
            {
                // Send our request and download the response.
                var s_SerializedRequest = JsonConvert.SerializeObject(p_Request, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                var s_URL = Library.BaseURL + "/more.php?" + p_Method;

                if (p_UseHTTPS)
                    s_URL = s_URL.Replace("http://", "https://");

                s_ResponseData = p_Client.UploadString(s_URL + p_Method, s_SerializedRequest);
            }
            catch
            {
                return default(Z);
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
                return default(Z);
            } 
        }

        private String DetermineSecretKey(String p_Method)
        {
            switch (p_Method)
            {
                /*case "getStreamKeysFromSongIDs":
                case "getStreamKeyFromFileToken":
                case "getStreamKeyFromSongIDEx":
                case "markSongDownloadedEx":
                case "markSongQueueSongPlayed":
                case "markStreamKeyOver30Seconds":
                case "getQueueSongListFromSongIDs":
                case "removeSongsFromQueue":
                case "addSongsToQueue":
                case "getTokens":
                    return m_SecretKey01;*/
                    
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
