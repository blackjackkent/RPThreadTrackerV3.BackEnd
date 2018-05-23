﻿namespace RPThreadTrackerV3.Infrastructure.Exceptions.Public
{
    using System;

    public class PublicViewSlugExistsException : Exception
    {
		public PublicViewSlugExistsException() : base("The supplied public view slug already exists.") { }
    }
}