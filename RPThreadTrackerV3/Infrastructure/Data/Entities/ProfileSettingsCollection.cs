namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using Interfaces.Data;
    public class ProfileSettingsCollection : IEntity
    {
		public int SettingsId { get; set; }
		public string UserId { get; set; }
		public bool ShowDashboardThreadDistribution { get; set; }
		public bool UseInvertedTheme { get; set; }
		public bool AllowMarkQueued { get; set; }
    }
}
