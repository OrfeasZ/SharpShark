using System;
using GS.Lib.Components;
using GS.Lib.Enums;
using GS.Lib.Network.HTTP;

namespace GS.Lib
{
    public class SharpShark
    {
        internal RequestDispatcher RequestDispatcher { get; private set; }

        internal AuthenticatorComponent Authenticator { get; private set; }
        internal ChatComponent Chat { get; private set; }

        public String BaseURL { get; private set; }

        public SharpShark(String p_SecretKey01, String p_SecretKey02)
        {
            BaseURL = "http://grooveshark.com";
            RequestDispatcher = new RequestDispatcher(this, p_SecretKey01, p_SecretKey02);

            InitComponents();
        }

        public SharpShark(String p_SecretKey01, String p_SecretKey02, String p_BaseURL)
        {
            BaseURL = p_BaseURL;
            RequestDispatcher = new RequestDispatcher(this, p_SecretKey01, p_SecretKey02);

            InitComponents();
        }

        private void InitComponents()
        {
            Authenticator = new AuthenticatorComponent(this);
            Chat = new ChatComponent(this);
        }

        public AuthenticationResult Authenticate(String p_Username, String p_Password)
        {
            return Authenticator.Authenticate(p_Username, p_Password);
        }

        public AuthenticationResult Authenticate(String p_SessionID)
        {
            return Authenticator.Authenticate(p_SessionID);
        }

        public bool ConnectChat()
        {
            if (Authenticator.User == null)
                return false;

            return Chat.Connect();
        }
    }
}
