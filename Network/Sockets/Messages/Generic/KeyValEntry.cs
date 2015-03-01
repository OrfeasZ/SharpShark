using System;

namespace GS.Lib.Network.Sockets.Messages.Generic
{
    internal class KeyValEntry<T> : SharkObject
    {
        public String Key { get; set; }

        public T Value { get; set; }

        public String Readable { get; set; }

        public KeyValEntry(String p_Key, T p_Value, String p_Readable = "global")
        {
            Key = p_Key;
            Value = p_Value;
            Readable = p_Readable;
        }
    }
}
