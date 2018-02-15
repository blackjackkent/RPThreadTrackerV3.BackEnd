namespace RPThreadTrackerV3.Models.DomainModels
{
	using System;

	public class ProfileSettings
    {
	    public int SettingsId { get; set; }
	    public string UserId { get; set; }
	    public bool ShowDashboardThreadDistribution { get; set; }
	    public bool UseInvertedTheme { get; set; }
	    public bool AllowMarkQueued { get; set; }
		public DateTime? LastNewsReadDate { get; set; }
	}
}
