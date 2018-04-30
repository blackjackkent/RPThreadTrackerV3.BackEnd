namespace RPThreadTrackerV3.Infrastructure.Exceptions.Thread
{
    using System;

    public class ThreadNotFoundException : Exception
    {
		public ThreadNotFoundException() : base("The requested thread does not exist for the current user.") { }
    }
}
