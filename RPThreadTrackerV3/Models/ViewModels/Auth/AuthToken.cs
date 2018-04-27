using System;

namespace RPThreadTrackerV3.Models.ViewModels.Auth
{
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
