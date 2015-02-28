using System;

namespace GS.Lib.Network.HTTP.Responses
{
    internal class SharkResponse<T>
        where T : SharkObject
    {
        internal class ResponseHeader
        {
            public String SessionID { get; set; }
            public String ServiceVersion { get; set; }
            public bool PrefetchEnabled { get; set; }
        }

        public ResponseHeader Header { get; set; }
        public T Result { get; set; }
    }
}
