namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;

	public class InvalidCharacterException : Exception
    {
		public InvalidCharacterException() : base("The supplied character is invalid.") { }
    }
}
