namespace RPThreadTrackerV3.Infrastructure.Exceptions.PublicViews
{
    using System;

    public class PublicViewSlugExistsException : Exception
    {
		public PublicViewSlugExistsException()
		    : base("The supplied public view slug already exists.") { }
    }
}
