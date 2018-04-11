﻿namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;

	public class PublicViewNotFoundException : Exception
    {
		public PublicViewNotFoundException() : base("The requested public view configuration does not exist for the current user.") { }
    }
}
