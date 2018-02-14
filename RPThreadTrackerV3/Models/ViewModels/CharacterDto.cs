namespace RPThreadTrackerV3.Models.ViewModels
{
	using Infrastructure.Enums;

	public class CharacterDto
	{
		public int CharacterId { get; set; }
		public string UserId { get; set; }
		public string UrlIdentifier { get; set; }
		public bool IsOnHiatus { get; set; }
		public Platform PlatformId { get; set; }
		public string HomeUrl { get; set; }
	}
}
