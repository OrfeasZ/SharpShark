namespace GS.Lib.Network.Sockets.Messages.Requests
{
    internal class ParameterizedRequest<T> : SharkMessage
    {
        public T Params { get; set; }

        public ParameterizedRequest(string p_Command, T p_Params) 
            : base(p_Command)
        {
            Params = p_Params;
        }
    }
}
