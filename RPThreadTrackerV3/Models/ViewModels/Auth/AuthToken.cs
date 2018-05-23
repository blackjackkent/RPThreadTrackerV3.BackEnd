namespace RPThreadTrackerV3.Models.ViewModels.Auth
{
    using System;

    public class AuthToken
    {
        public AuthToken(string token, DateTime expiry)
        {
            Token = token;
            Expiry = expiry.Ticks;
        }

        public string Token { get; set; }
        public long Expiry { get; set; }
    }
}
