﻿using System;
using System.Collections.Generic;

namespace GS.Lib.Components
{
    public partial class ChatComponent
    {
        private void PingMaster()
        {
            // TODO: Implement timeout timer
            BroadcastMessageToSelf("masterPing");
        }
    }
}
