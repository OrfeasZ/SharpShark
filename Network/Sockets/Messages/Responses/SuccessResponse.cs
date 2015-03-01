namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SuccessResponse<T, Z> : BasicResponse
    {
        public T Success { get; set; }

        public Z Blackbox { get; set; }

        public SuccessResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
