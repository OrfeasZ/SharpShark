namespace GS.Lib.Network.Sockets.Messages.Responses
{
    internal class SuccessResponse<T> : BasicResponse
    {
        public T Success { get; set; }

        public SuccessResponse(string p_Command)
            : base(p_Command)
        {
        }
    }
}
