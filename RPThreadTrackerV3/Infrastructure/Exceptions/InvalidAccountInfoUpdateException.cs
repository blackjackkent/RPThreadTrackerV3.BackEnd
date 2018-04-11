namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;
	using System.Collections.Generic;

	public class InvalidAccountInfoUpdateException : Exception
    {
	    public List<string> Errors { get; }

	    public InvalidAccountInfoUpdateException(List<string> errors) : base("There was an error updating the users's account information.") {
		    Errors = errors;
	    }
	}
}
