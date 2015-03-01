using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using GS.Lib.Models;
using GS.Lib.Network.Sockets.Messages;
using GS.Lib.Network.Sockets.Messages.Responses;
using GS.Lib.Util;
using Newtonsoft.Json.Linq;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private const  UInt32 c_MaxOnlineIdleTime = 900 * 1000;

        private void HandleGet(SharkResponseMessage p_Message)
        {
            var s_BlackboxResponse = p_Message.As<BlackboxResponse>();

            if (s_BlackboxResponse != null && s_BlackboxResponse.Blackbox != null && s_BlackboxResponse.Blackbox.ContainsKey("source") &&
                (String) s_BlackboxResponse.Blackbox["source"] == "broadcast")
            {
                HandleGetBroadcast(p_Message);
                return;
            }

            var s_ReturnResponse = p_Message.As<ReturnResponse>();

            if (s_ReturnResponse != null && s_ReturnResponse.Return != null &&
                s_ReturnResponse.Return.Value<JArray>("values") != null)
            {
                HandleGetReturn(p_Message);
                return;
            }

            if (s_BlackboxResponse != null && s_BlackboxResponse.Blackbox != null &&
                s_BlackboxResponse.Blackbox.ContainsKey("source") &&
                (String) s_BlackboxResponse.Blackbox["source"] == "restore")
            {
                HandleGetRestore(p_Message);
                return;
            }

        }

        private void HandleGetBroadcast(SharkResponseMessage p_Message)
        {
            // TODO: Implement events
        }

        private void HandleGetReturn(SharkResponseMessage p_Message)
        {
            var s_Message = p_Message.As<ReturnResponse>();

            var s_Values = s_Message.Return.Value<JArray>("values");

            if (s_Message.Blackbox != null && s_Message.Blackbox.ContainsKey("source") &&
                (String) s_Message.Blackbox["source"] == "restore")
            {
                m_ReceivedRestore = true;

                if (s_Values != null && s_Values.Count > 0 && s_Values[0] != null)
                {
                    var s_Value = s_Values[0].ToObject<Dictionary<String, Object>>();

                    Object s_Remora, s_Bcast, s_BcastOwner, s_Status, s_Time, s_SongEx;

                    s_Value.TryGetValue("remora", out s_Remora);
                    s_Value.TryGetValue("bcast", out s_Bcast);
                    s_Value.TryGetValue("bcastOwner", out s_BcastOwner);
                    s_Value.TryGetValue("status", out s_Status);
                    s_Value.TryGetValue("time", out s_Time);
                    s_Value.TryGetValue("songEx", out s_SongEx);

                    if ((s_Remora == null || s_BcastOwner == null) &&
                        (s_Status == null ||
                         (s_Time != null &&
                          ((DateTime.UtcNow.ToUnixTimestamp()*1000) - (Int64) s_Time) > c_MaxOnlineIdleTime ||
                          s_SongEx == null)))
                    {
                        PromoteSelfToMaster("last_master_idle_lower");
                    }
                    else
                    {
                        var s_ConvertedTime = (Int64) (s_Time ?? DateTime.UtcNow.ToUnixTimestamp()*1000);

                        LastMasterUUID = LoggedInMaster != null ? LoggedInMaster.UUID : null;
                        LoggedInMaster = new MasterStatus()
                        {
                            UUID = null,
                            LastMouseMove = (uint) s_ConvertedTime,
                            LastUpdate =
                                (uint) ((DateTime.UtcNow.ToUnixTimestamp()*1000) -
                                        ((DateTime.UtcNow.ToUnixTimestamp()*1000) - s_ConvertedTime)),
                            CurrentBroadcast = s_Bcast as String,
                            IsBroadcasting = s_BcastOwner != null && (int) s_BcastOwner == 1,
                            CurrentlyPlayingSong = s_SongEx != null
                        };

                        if ((DateTime.UtcNow.ToUnixTimestamp()*1000) - s_ConvertedTime > 5*60*1000 ||
                            (DateTime.UtcNow.ToUnixTimestamp()*1000) - s_ConvertedTime - s_ConvertedTime > 10*1000 &&
                            s_Remora == null)
                            PingMaster();
                        else if (LastMasterUUID != null)
                            PromoteSelfToMaster("were_last_master");
                    }
                }
                else
                {
                    PromoteSelfToMaster("restore_not_found");
                }
            }
        }

        private void HandleGetRestore(SharkResponseMessage p_Message)
        {
            
        }
    }
}
