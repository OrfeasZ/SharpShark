﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using GS.Lib.Enums;
using GS.Lib.Models;
using GS.Lib.Network.HTTP.Requests;
using GS.Lib.Network.HTTP.Responses;
using GS.Lib.Util;
using Newtonsoft.Json;

namespace GS.Lib.Components
{
    public class UserComponent : SharkComponent
    {
        public String SessionID { get; set; }

        public String CommunicationToken { get; set; }

        public Guid UUID { get; set; }

        public UserData User { get; set; }

        internal UserComponent(SharpShark p_Library)
            : base(p_Library)
        {
            User = null;
            UUID = Guid.Empty;
            SessionID = CommunicationToken = null;
        }

        public AuthenticationResult Authenticate(String p_SessionID)
        {
            User = null;
            UUID = Guid.Empty;
            SessionID = CommunicationToken = null;

            // Try to fetch token data.
            var s_TokenData = FetchTokenData(p_SessionID);
            
            // Check if we have the data we need.
            if (s_TokenData == null || s_TokenData.GetGSConfig == null || s_TokenData.GetGSConfig.User == null)
                return AuthenticationResult.NoTokenData;

            // Check if the user was successfully logged in.
            if (String.IsNullOrWhiteSpace(s_TokenData.GetGSConfig.User.Email))
                return AuthenticationResult.InvalidSession;

            // Store required information.
            CommunicationToken = s_TokenData.GetCommunicationToken;
            User = s_TokenData.GetGSConfig.User;
            SessionID = s_TokenData.GetGSConfig.SessionID;
            UUID = s_TokenData.GetGSConfig.UUID;
            Library.Chat.ChatServers = s_TokenData.GetGSConfig.ChatServersWeighted;

            return AuthenticationResult.Success;
        }

        public AuthenticationResult Authenticate(String p_Username, String p_Password)
        {
            User = null;
            UUID = Guid.Empty;
            SessionID = CommunicationToken = null;

            // Try to fetch token data.
            var s_TokenData = FetchTokenData();

            if (s_TokenData == null || s_TokenData.GetGSConfig == null)
                return AuthenticationResult.NoTokenData;

            // Store required information.
            CommunicationToken = s_TokenData.GetCommunicationToken;
            SessionID = s_TokenData.GetGSConfig.SessionID;
            UUID = s_TokenData.GetGSConfig.UUID;
            Library.Chat.ChatServers = s_TokenData.GetGSConfig.ChatServersWeighted;

            var s_Request = new AuthenticationRequest
            {
                Username = p_Username, 
                Password = p_Password
            };

            // Send the authentication request.
            var s_Response = Library.RequestDispatcher.Dispatch<AuthenticationRequest, AuthenticationResponse>("authenticateUser", s_Request);

            if (s_Response == null)
                return AuthenticationResult.InternalError;

            if (String.IsNullOrWhiteSpace(s_Response.Email))
                return AuthenticationResult.InvalidCredentials;

            User = s_Response;

            return AuthenticationResult.Success;
        }

        private TokenData FetchTokenData(String p_SessionID = null)
        {
            // Initialize our WebClient and set our SessionID (if we have one).
            using (var s_Client = new WebClient())
            {
                if (!String.IsNullOrWhiteSpace(p_SessionID))
                    s_Client.Headers.Add(HttpRequestHeader.Cookie, "PHPSESSID=" + p_SessionID);

                string s_Data;

                // Catch web exceptions.
                // TODO: Better handing here?
                try
                {
                    s_Data = s_Client.DownloadString(Library.BaseURL + "/preload.php?getCommunicationToken=1&hash=&" + 
                                                DateTime.UtcNow.ToUnixTimestamp());
                }
                catch
                {
                    return null;
                }

                // Try to extract the tokenData from the response.
                var s_Match = Regex.Match(s_Data, @"window\.tokenData = ({.*});", RegexOptions.Singleline);

                if (!s_Match.Success)
                    return null;

                // Deserialize the tokenData.
                try
                {
                    return JsonConvert.DeserializeObject<TokenData>(s_Match.Groups[1].Value);
                }
                catch
                {
                    throw new Exception("Authentication failed; has GS changed something?");
                }
            }
        }

        public List<Int64> GetCollectionSongs()
        {
            if (User == null)
                return new List<long>();

            var s_Response = Library.RequestDispatcher.Dispatch<Dictionary<String, String>, LibrarySongIDsResponse>(
                "userGetSongIDsInLibrary", new Dictionary<String, String>());

            if (s_Response == null)
                return new List<long>();

            return s_Response.SongIDs;
        }

        public List<Int64> GetFavoriteSongs()
        {
            if (User == null)
                return new List<long>();

            var s_Response = Library.RequestDispatcher.Dispatch<Dictionary<String, String>, List<Int64>>(
                "userGetFavoriteSongsSongIDs", new Dictionary<String, String>());

            if (s_Response == null)
                return new List<long>();

            return s_Response;
        }

        public List<SongResultData> GetFavorites(String p_What, Int64 p_UserID)
        {
            var s_Request = new GetFavoritesRequest 
            { 
                OfWhat = p_What, 
                UserID = p_UserID 
            };

            var s_Response = Library.RequestDispatcher.Dispatch<GetFavoritesRequest, List<SongResultData>>("getFavorites", s_Request);

            if (s_Response == null)
                return new List<SongResultData>();

            return s_Response;
        }
    }
}