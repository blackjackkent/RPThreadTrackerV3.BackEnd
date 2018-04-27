using System;

namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException() : base("The supplied refresh token is invalid.") { }
    }
}
