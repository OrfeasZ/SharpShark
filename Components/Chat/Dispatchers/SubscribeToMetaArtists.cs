using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Network.Sockets.Messages.Requests;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private List<String> m_SubscribedMetaArtistIDs;

        private void SubscribeToMetaArtists(List<String> p_ArtistIDs, bool p_Replace, bool p_InitialSubscribe)
        {
            if (m_SubscribedMetaArtistIDs == null || p_Replace)
            {
                m_SubscribedMetaArtistIDs = p_ArtistIDs;
            }
            else
            {
                foreach (var s_UserID in p_ArtistIDs)
                    if (!m_SubscribedMetaArtistIDs.Contains(s_UserID))
                        m_SubscribedMetaArtistIDs.Add(s_UserID);
            }

            if (m_SubscribedMetaArtistIDs.Count == 0)
                return;

            foreach (var s_Request in FormSubscribeToMetaArtistsRequest(false, p_InitialSubscribe))
                m_SocketClient.SendMessage(s_Request);
        }

        private IEnumerable<SubscribeToMetaArtistsRequest> FormSubscribeToMetaArtistsRequest(bool p_Retried, bool p_InitialSubscribe)
        {
            var s_Parts = m_SubscribedMetaArtistIDs
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / c_MaxIDsPerCall)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            for (var i = 0; i < s_Parts.Count; ++i)
            {
                var s_Request = new SubscribeToMetaArtistsRequest(s_Parts[i], s_Parts.Count, i + 1, p_InitialSubscribe, p_Retried);
                yield return s_Request;
            }
        }
    }
}
