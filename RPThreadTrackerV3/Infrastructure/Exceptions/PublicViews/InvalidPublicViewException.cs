namespace RPThreadTrackerV3.Infrastructure.Exceptions.PublicViews
{
    using System;

    public class InvalidPublicViewException : Exception
    {
		public InvalidPublicViewException()
		    : base("The supplied public view configuration is invalid.") { }
    }
}
