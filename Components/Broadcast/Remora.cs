using System;
using System.Collections.Generic;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        internal Dictionary<String, Object> GetParamsForRemora()
        {
            var s_Tag = CurrentBroadcastCategoryTag ?? new CategoryTag(0, "multi-genre");

            return new Dictionary<string, object>()
            {
                { "broadcastID", ActiveBroadcastID },
                { "userID", Library.User.Data.UserID },
                { "name", CurrentBroadcastName },
                { "description", CurrentBroadcastDescription },
                { "image", CurrentBroadcastPicture ?? "" },
                { "privacy", 0 }, // TODO: Privacy
                { "tag", s_Tag },
                { "version", 4 },
                { "vipUsers", new List<Int64>() }, // TODO: VIP Users 
                { "bannedUserIDs", new List<Int64>() }, // TODO: Banned Users
                { "attachType", "user" },
                { "attachID", Library.User.Data.UserID },
                { "settings", 
                    new Dictionary<String, bool>
                    {
                        {  "suggestionsEnabled", SuggestionsEnabled },
                        {  "chatEnabled", ChatEnabled }
                    } 
                },
                { "isBroadcast", true },
                { "isRandomName", false },
                { "clientVersion", "1.1" },
            };
        }
    }
}
