namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;

	public class InvalidThreadException : Exception
    {
		public InvalidThreadException() : base("The supplied thread was invalid.") { }
    }
}
