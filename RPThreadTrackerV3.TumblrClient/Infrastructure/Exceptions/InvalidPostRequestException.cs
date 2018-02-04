namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Exceptions
{
	using System;

	public class InvalidPostRequestException : Exception
    {
		public InvalidPostRequestException() : base("Invalid request for post information.") { }
    }
}
