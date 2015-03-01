using System;

namespace GS.Lib.Network.HTTP.Requests
{
    internal class GetAutocompleteRequest : SharkObject
    {
        public String Query { get; set; }
        public String Type { get; set; }
    }
}
