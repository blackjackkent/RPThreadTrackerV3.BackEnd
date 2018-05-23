namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    public class InvalidAccountInfoUpdateException : Exception
    {
	    public List<string> Errors { get; }

        /// <inheritdoc />
        public InvalidAccountInfoUpdateException(List<string> errors)
            : base("There was an error updating the users's account information.")
        {
		    Errors = errors;
	    }
	}
}
