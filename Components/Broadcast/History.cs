using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public partial class BroadcastComponent
    {
        private Dictionary<String, Object> GetChatVariableHistory()
        {
            // TODO: Implement
            return new Dictionary<string, object>()
            {
                { "s", new List<object>() }
            };
        }
    }
}
