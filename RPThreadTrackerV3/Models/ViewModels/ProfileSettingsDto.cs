namespace RPThreadTrackerV3.Models.ViewModels
{
    public class ProfileSettingsDto
    {
	    public int SettingsId { get; set; }
	    public string UserId { get; set; }
	    public bool ShowDashboardThreadDistribution { get; set; }
	    public bool UseInvertedTheme { get; set; }
	    public bool AllowMarkQueued { get; set; }
	}
}
