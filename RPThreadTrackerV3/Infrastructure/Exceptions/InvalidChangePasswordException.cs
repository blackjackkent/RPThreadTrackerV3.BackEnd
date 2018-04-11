namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;
	using System.Collections.Generic;

	public class InvalidChangePasswordException : Exception
    {
	    public List<string> Errors { get; }

	    public InvalidChangePasswordException(List<string> errors) : base("There was an error changing the user's password.")
	    {
		    Errors = errors;
	    }
	}
}
