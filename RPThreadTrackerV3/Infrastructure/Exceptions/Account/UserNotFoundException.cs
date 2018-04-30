namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;

    public class UserNotFoundException : Exception
    {
	    public UserNotFoundException() : base("The requested user does not exist.") { }
	}
}
