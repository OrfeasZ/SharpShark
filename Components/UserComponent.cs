using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public UserData Data { get; set; }

        public CountryData CountryData { get; set; }

        internal UserComponent(SharpShark p_Library)
            : base(p_Library)
        {
            Data = null;
            CountryData = null;
            SessionID = CommunicationToken = null;
        }

        public AuthenticationResult Authenticate(String p_SessionID)
        {
            Data = null;
            CountryData = null;
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
            Data = s_TokenData.GetGSConfig.User;
            SessionID = s_TokenData.GetGSConfig.SessionID;
            Library.Chat.ChatServers = s_TokenData.GetGSConfig.ChatServersWeighted;
            CountryData = s_TokenData.GetGSConfig.Country;
            Library.Remora.Channel = s_TokenData.GetGSConfig.RemoraChannel;
            Library.TimeDifference = s_TokenData.GetGSConfig.Timestamp * 1000 - DateTime.UtcNow.ToUnixTimestampMillis();

            Debug.WriteLine("Time difference calculated to {0}ms.", Library.TimeDifference);

            return AuthenticationResult.Success;
        }

        public AuthenticationResult Authenticate(String p_Username, String p_Password)
        {
            Data = null;
            CountryData = null;
            SessionID = CommunicationToken = null;

            // Try to fetch token data.
            var s_TokenData = FetchTokenData();

            if (s_TokenData == null || s_TokenData.GetGSConfig == null)
                return AuthenticationResult.NoTokenData;

            // Store required information.
            CommunicationToken = s_TokenData.GetCommunicationToken;
            SessionID = s_TokenData.GetGSConfig.SessionID;
            Library.Chat.ChatServers = s_TokenData.GetGSConfig.ChatServersWeighted;
            CountryData = s_TokenData.GetGSConfig.Country;
            Library.Remora.Channel = s_TokenData.GetGSConfig.RemoraChannel;
            Library.TimeDifference = s_TokenData.GetGSConfig.Timestamp * 1000 - DateTime.UtcNow.ToUnixTimestampMillis();

            Debug.WriteLine("Time difference calculated to {0}ms.", Library.TimeDifference);

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

            Data = s_Response;

            return AuthenticationResult.Success;
        }

        public bool InitSession()
        {
            Data = null;
            CountryData = null;
            SessionID = CommunicationToken = null;

            // Try to fetch token data.
            var s_TokenData = FetchTokenData();

            if (s_TokenData == null || s_TokenData.GetGSConfig == null)
                return false;

            // Store required information.
            CommunicationToken = s_TokenData.GetCommunicationToken;
            SessionID = s_TokenData.GetGSConfig.SessionID;
            Library.Chat.ChatServers = s_TokenData.GetGSConfig.ChatServersWeighted;
            CountryData = s_TokenData.GetGSConfig.Country;
            Library.Remora.Channel = s_TokenData.GetGSConfig.RemoraChannel;
            Library.TimeDifference = s_TokenData.GetGSConfig.Timestamp * 1000 - DateTime.UtcNow.ToUnixTimestampMillis();

            Debug.WriteLine("Time difference calculated to {0}ms.", Library.TimeDifference);

            return true;
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
                                                DateTime.UtcNow.ToUnixTimestampMillis());
                }
                catch
                {
                    return null;
                }

                // Try to extract the tokenData from the response.
                var s_Match = Regex.Match(s_Data, @"window\.tokenData = (\{.*\});", RegexOptions.Singleline);

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

        public List<ResultData> GetSongsInLibrary(Int64 p_UserID)
        {
            if (SessionID == null)
                return new List<ResultData>();

            var s_Response = Library.RequestDispatcher.Dispatch<UserGetSongsInLibraryRequest, UserGetSongsInLibraryResponse>("userGetSongsInLibrary",
                new UserGetSongsInLibraryRequest() { Page = 0, UserID = p_UserID });

            if (s_Response == null || s_Response.Songs == null)
                return new List<ResultData>();

            return s_Response.Songs;
        }

        public List<Int64> GetCollectionSongs()
        {
            if (Data == null)
                return new List<long>();

            var s_Response = Library.RequestDispatcher.Dispatch<Object, LibrarySongIDsResponse>(
                "userGetSongIDsInLibrary", null);

            if (s_Response == null)
                return new List<long>();

            return s_Response.SongIDs;
        }

        public List<Int64> GetFavoriteSongs()
        {
            if (Data == null)
                return new List<long>();

            var s_Response = Library.RequestDispatcher.Dispatch<Object, List<Int64>>(
                "userGetFavoriteSongsSongIDs", null);

            if (s_Response == null)
                return new List<long>();

            return s_Response;
        }

        public List<ResultData> GetFavorites(String p_What, Int64 p_UserID)
        {
            if (String.IsNullOrWhiteSpace(SessionID))
                return new List<ResultData>();

            var s_Request = new GetFavoritesRequest 
            { 
                OfWhat = p_What, 
                UserID = p_UserID 
            };

            var s_Response = Library.RequestDispatcher.Dispatch<GetFavoritesRequest, List<ResultData>>("getFavorites", s_Request);

            if (s_Response == null)
                return new List<ResultData>();

            return s_Response;
        }

        public List<FollowerFollowingUserData> GetFollowers()
        {
            if (Data == null)
                return new List<FollowerFollowingUserData>();

            var s_Response = Library.RequestDispatcher.Dispatch<Object, List<FollowerFollowingUserData>>(
                    "userGetFollowersFollowingEx", null);

            var s_Users = new List<FollowerFollowingUserData>();

            if (s_Response == null)
                return s_Users;

            s_Users.AddRange(s_Response.Where(p_User => !String.IsNullOrWhiteSpace(p_User.IsFollower) && p_User.IsFollower == "1"));

            return s_Users;
        }

        public List<FollowerFollowingUserData> GetFollowingUsers()
        {
            if (Data == null)
                return new List<FollowerFollowingUserData>();

            var s_Response = Library.RequestDispatcher.Dispatch<Object, List<FollowerFollowingUserData>>(
                    "userGetFollowersFollowingEx", null);

            var s_Users = new List<FollowerFollowingUserData>();

            if (s_Response == null)
                return s_Users;

            s_Users.AddRange(s_Response.Where(
                    p_User =>
                        !String.IsNullOrWhiteSpace(p_User.FollowingFlags) && p_User.FollowingFlags.Length > 0 &&
                        (String.IsNullOrWhiteSpace(p_User.IsArtist) || p_User.IsArtist == "0")));

            return s_Users;
        }

        public List<FollowerFollowingUserData> GetFollowingArtists()
        {
            if (Data == null)
                return new List<FollowerFollowingUserData>();

            var s_Response = Library.RequestDispatcher.Dispatch<Object, List<FollowerFollowingUserData>>(
                    "userGetFollowersFollowingEx", null);

            var s_Users = new List<FollowerFollowingUserData>();

            if (s_Response == null)
                return s_Users;

            s_Users.AddRange(s_Response.Where(
                    p_User =>
                        !String.IsNullOrWhiteSpace(p_User.FollowingFlags) && p_User.FollowingFlags.Length > 0 &&
                        (String.IsNullOrWhiteSpace(p_User.IsArtist) || p_User.IsArtist == "1")));

            return s_Users;
        }

        internal void StoreChatIdentity()
        {
            Library.RequestDispatcher.Dispatch<Object, Object>("storeChatIdentity", null);
        }

        public void AddSongToLibrary(Int64 p_SongID)
        {
            var s_Response = Library.RequestDispatcher.Dispatch<Dictionary<String, Object>, Object>("userAddSongsToLibrary", new Dictionary<String, object>()
            {
                { "songs", new List<Object>
                {
                    new
                    {
                        songID = p_SongID
                    }
                }}
            });
        }

        public void RemoveSongFromLibrary(Int64 p_SongID)
        {
            var s_Response = Library.RequestDispatcher.Dispatch<Dictionary<String, Object>, Object>("userAddSongsToLibrary", new Dictionary<String, object>()
            {
                { "songIDs", new List<Int64> { p_SongID } },
                { "userID", Library.User.Data.UserID }
            });
        }

        public List<PlaylistData> GetPlaylists(Int64 p_UserID)
        {
            var s_Response = Library.RequestDispatcher.Dispatch<UserGetPlaylistsRequest, UserGetPlaylistsResponse>("userGetPlaylists",  new UserGetPlaylistsRequest()
            {
                UserID = p_UserID
            });

            if (s_Response != null)
                return s_Response.Playlists;

            return new List<PlaylistData>();
        }

        public GetPlaylistByIDResponse GetPlaylistData(Int64 p_PlaylistID)
        {
            return Library.RequestDispatcher.Dispatch<GetPlaylistByIDRequest, GetPlaylistByIDResponse>("getPlaylistByID", 
                new GetPlaylistByIDRequest(p_PlaylistID));
        }

    }
}
