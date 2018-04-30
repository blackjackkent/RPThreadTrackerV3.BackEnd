namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    public class InvalidPasswordResetException : Exception
	{
		public List<string> Errors { get; }

		public InvalidPasswordResetException(List<string> errors) : base("There was an error resetting the user's password.")
		{
			Errors = errors;
		}
	}
}

