namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;

    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException() : base("The supplied refresh token is invalid.") { }
    }
}
