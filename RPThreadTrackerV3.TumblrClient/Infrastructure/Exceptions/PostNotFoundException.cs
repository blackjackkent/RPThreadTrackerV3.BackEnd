namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Exceptions
{
	using System;

	public class PostNotFoundException : Exception
    {
		public PostNotFoundException() : base("The requested post does not exist.") { }
    }
}
