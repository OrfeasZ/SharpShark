namespace GS.Lib.Network.Sockets.Messages.Generic
{
    internal class KeyValParams<T> : SharkObject
    {
        public KeyVals<T> Keyvals { get; set; }

        public KeyValParams()
        {
            Keyvals = new KeyVals<T>();
        }
    }
}
