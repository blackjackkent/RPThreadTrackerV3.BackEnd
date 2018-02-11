namespace RPThreadTrackerV3.Infrastructure.Exceptions
{
	using System;

	public class ProfileSettingsNotFoundException : Exception
    {
		public ProfileSettingsNotFoundException() : base("No profile settings exist for the current user.") { }
    }
}
