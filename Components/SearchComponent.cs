using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.HTTP.Requests;
using GS.Lib.Network.HTTP.Responses;

namespace GS.Lib.Components
{
    public class SearchComponent : SharkComponent
    {
        public SearchComponent(SharpShark p_Library) 
            : base(p_Library)
        {
        }

        public Dictionary<String, List<SongResultData>> GetResultsFromSearch(String p_Query, IEnumerable<String> p_Types)
        {
            var s_Request = new GetResultsFromSearchRequest()
            {
                Guts = 0,
                PpOverride = false,
                Query = p_Query,
                Type = p_Types.ToList()
            };

            var s_Response = Library.RequestDispatcher.Dispatch<GetResultsFromSearchRequest, GetResultsFromSearchResponse>(
                "getResultsFromSearch", s_Request);

            return s_Response == null
                ? s_Request.Type.ToDictionary(p_Type => p_Type, p_Type => new List<SongResultData>())
                : s_Response.Result;
        }
    }
}
