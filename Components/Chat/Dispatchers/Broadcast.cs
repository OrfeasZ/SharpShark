using System;
using System.Collections.Generic;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void BroadcastMessageToSelf(String p_Type)
        {
            BroadcastMessageToSelf(new Dictionary<string, string>(), p_Type);
        }

        private void BroadcastMessageToSelf<T>(T p_Params, String p_Type)
        {
            if (Library.User.Data == null)
                return;

            if (p_Params == null)
            {
                BroadcastMessageToSelf(new Dictionary<string, string>(), p_Type);
                return;
            }

            if (Library.User.Data.ArtistID != 0)
            {
                PublishToChannels(new List<string>() { "artist:" + Library.User.Data.ArtistID }, new Dictionary<String, Object>()
                {
                    { "type", p_Type },
                    { "params", p_Params },
                    { "sessionPart", GetSessionPart() }
                });
            }
            else if (Library.User.Data.UserID != 0)
            {
                PublishToChannels(new List<string>() { "user:" + Library.User.Data.UserID }, new Dictionary<String, Object>()
                {
                    { "type", p_Type },
                    { "params", p_Params },
                    { "sessionPart", GetSessionPart() }
                });
            }
        }
    }
}
