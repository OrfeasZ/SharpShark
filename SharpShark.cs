using System;
using GS.Lib.Components;
using GS.Lib.Network.HTTP;

namespace GS.Lib
{
    public class SharpShark
    {
        internal RequestDispatcher RequestDispatcher { get; private set; }

        public UserComponent User { get; private set; }
        public ChatComponent Chat { get; private set; }
        public BroadcastComponent Broadcast { get; private set; }
        public SearchComponent Search { get; private set; }
        public SongsComponent Songs { get; private set; }

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
            User = new UserComponent(this);
            Chat = new ChatComponent(this);
            Broadcast = new BroadcastComponent(this);
            Search = new SearchComponent(this);
            Songs = new SongsComponent(this);
        }
    }
}
