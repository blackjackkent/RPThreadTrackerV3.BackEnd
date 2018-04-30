namespace RPThreadTrackerV3.Infrastructure.Exceptions.Characters
{
    using System;

    public class InvalidCharacterException : Exception
    {
		public InvalidCharacterException() : base("The supplied character is invalid.") { }
    }
}
