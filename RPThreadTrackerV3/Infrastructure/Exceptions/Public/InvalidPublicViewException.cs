namespace RPThreadTrackerV3.Infrastructure.Exceptions.Public
{
    using System;

    public class InvalidPublicViewException : Exception
    {
		public InvalidPublicViewException() : base("The supplied public view configuration is invalid.") { }
    }
}
