namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using System.ComponentModel.DataAnnotations.Schema;
	using Interfaces.Data;
    public class ProfileSettingsCollection : IEntity
    {
		[Column("SettingsId")]
		public int ProfileSettingsCollectionId { get; set; }
		public string UserId { get; set; }
		public bool ShowDashboardThreadDistribution { get; set; }
		public bool UseInvertedTheme { get; set; }
		public bool AllowMarkQueued { get; set; }
    }
}
