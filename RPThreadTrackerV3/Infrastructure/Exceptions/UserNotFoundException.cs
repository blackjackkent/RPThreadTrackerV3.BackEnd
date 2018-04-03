namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;

	public class UserNotFoundException : Exception
    {
	    public UserNotFoundException() : base("The requested user does not exist.") { }
	}
}
