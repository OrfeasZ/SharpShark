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
        internal SearchComponent(SharpShark p_Library) 
            : base(p_Library)
        {
        }

        public Dictionary<String, List<SongResultData>> GetResultsFromSearch(String p_Query, IEnumerable<String> p_Types)
        {
            var s_Request = new GetResultsFromSearchRequest()
            {
                Guts = 0,
                PpOverride = "HTP4PopArtist",
                Query = p_Query,
                Type = p_Types.ToList()
            };

            var s_Response = Library.RequestDispatcher.Dispatch<GetResultsFromSearchRequest, GetResultsFromSearchResponse>(
                "getResultsFromSearch", s_Request);

            if (s_Response == null)
                return s_Request.Type.ToDictionary(p_Type => p_Type, p_Type => new List<SongResultData>());

            var s_Results = new Dictionary<String, List<SongResultData>>();

            foreach (var s_Type in s_Request.Type)
            {
                switch (s_Type)
                {
                    case "Songs":
                    {
                        if (s_Response.Result.Songs != null)
                        {
                            s_Results.Add("Songs", s_Response.Result.Songs);
                            break;
                        }

                        s_Results.Add("Songs", new List<SongResultData>());
                        break;
                    }

                    case "Artists":
                    {
                        if (s_Response.Result.Artists != null)
                        {
                            s_Results.Add("Artists", s_Response.Result.Artists);
                            break;
                        }

                        s_Results.Add("Artists", new List<SongResultData>());
                        break;
                    }

                    case "Albums":
                    {
                        if (s_Response.Result.Albums != null)
                        {
                            s_Results.Add("Albums", s_Response.Result.Albums);
                            break;
                        }

                        s_Results.Add("Albums", new List<SongResultData>());
                        break;
                    }

                    case "Videos":
                    {
                        if (s_Response.Result.Videos != null)
                        {
                            s_Results.Add("Videos", s_Response.Result.Videos);
                            break;
                        }

                        s_Results.Add("Videos", new List<SongResultData>());
                        break;
                    }

                    case "Playlists":
                    {
                        if (s_Response.Result.Playlists != null)
                        {
                            s_Results.Add("Playlists", s_Response.Result.Playlists);
                            break;
                        }

                        s_Results.Add("Playlists", new List<SongResultData>());
                        break;
                    }

                    case "Users":
                    {
                        if (s_Response.Result.Users != null)
                        {
                            s_Results.Add("Users", s_Response.Result.Users);
                            break;
                        }

                        s_Results.Add("Users", new List<SongResultData>());
                        break;
                    }

                    case "Events":
                    {
                        if (s_Response.Result.Events != null)
                        {
                            s_Results.Add("Events", s_Response.Result.Events);
                            break;
                        }

                        s_Results.Add("Events", new List<SongResultData>());
                        break;
                    }

                    default:
                    {
                        s_Results.Add(s_Type, new List<SongResultData>());
                        break;
                    }
                }
            }

            return s_Results;
        }

        public Dictionary<String, List<SongResultData>> GetAutocomplete(String p_Query, String p_Type = "combined")
        {
            var s_Request = new GetAutocompleteRequest
            {
                Query = p_Query,
                Type = p_Type
            };

            var s_Response = Library.RequestDispatcher.Dispatch<GetAutocompleteRequest, Dictionary<String, List<SongResultData>>>(
                    "getAutocompleteEx", s_Request);

            if (s_Response == null)
                return new Dictionary<String, List<SongResultData>>();

            return s_Response;
        }
    }
}
