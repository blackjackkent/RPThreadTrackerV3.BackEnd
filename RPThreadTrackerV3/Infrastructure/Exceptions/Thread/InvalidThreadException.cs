namespace RPThreadTrackerV3.Infrastructure.Exceptions.Thread
{
    using System;

    public class InvalidThreadException : Exception
    {
		public InvalidThreadException() : base("The supplied thread was invalid.") { }
    }
}
