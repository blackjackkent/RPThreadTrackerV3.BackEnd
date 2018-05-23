namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    public class InvalidRegistrationException : Exception
    {
        public List<string> Errors { get; }

        public InvalidRegistrationException(List<string> errors)
            : base("The supplied registration information is invalid.")
        {
            Errors = errors;
        }
    }
}
