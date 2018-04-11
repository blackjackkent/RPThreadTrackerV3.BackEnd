namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;

	public class PublicViewSlugExistsException : Exception
    {
		public PublicViewSlugExistsException() : base("The supplied public view slug already exists.") { }
    }
}
