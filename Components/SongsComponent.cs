using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.HTTP.Requests;

namespace GS.Lib.Components
{
    public class SongsComponent : SharkComponent
    {
        internal SongsComponent(SharpShark p_Library) 
            : base(p_Library)
        {
        }

        public StreamKeyData GetStreamKeyFromSongID(Int64 p_SongID, bool p_Prefetch = false, bool p_Mobile = false, Int64 p_Type = 0)
        {
            if (String.IsNullOrWhiteSpace(Library.User.SessionID))
                return null;

            var s_Request = new GetStreamKeyFromSongIDRequest
            {
                SongID = p_SongID,
                Prefetch = p_Prefetch,
                Mobile = p_Mobile,
                Type = p_Type
            };

            if (Library.User.CountryData != null)
                s_Request.Country = Library.User.CountryData;

            var s_Response = Library.RequestDispatcher.Dispatch<GetStreamKeyFromSongIDRequest, StreamKeyData>(
                "getStreamKeyFromSongIDEx", s_Request);

            return s_Response;
        }

        public Dictionary<Int64, StreamKeyData> GetStreamKeysFromSongIDs(IEnumerable<Int64> p_SongIDs, bool p_Prefetch = false, bool p_Mobile = false, Int64 p_Type = 0)
        {
            if (String.IsNullOrWhiteSpace(Library.User.SessionID))
                return new Dictionary<long, StreamKeyData>();

            var s_Request = new GetStreamKeysFromSongIDsRequest
            {
                SongIDs = p_SongIDs.ToList(),
                Prefetch = p_Prefetch,
                Mobile = p_Mobile,
                Type = p_Type
            };

            if (Library.User.CountryData != null)
                s_Request.Country = Library.User.CountryData;

            var s_Response = Library.RequestDispatcher.Dispatch<GetStreamKeysFromSongIDsRequest, Dictionary<String, StreamKeyData>>(
                "getStreamKeysFromSongIDs", s_Request);

            if (s_Response == null)
                return new Dictionary<long, StreamKeyData>();

            return s_Response.ToDictionary(p_Pair => Int64.Parse(p_Pair.Key), p_Pair => p_Pair.Value);
        }
    }
}
