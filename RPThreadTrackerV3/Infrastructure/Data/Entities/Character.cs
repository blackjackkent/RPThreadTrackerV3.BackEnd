namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using Interfaces.Data;
	using Microsoft.AspNetCore.Identity;

	public class Character : IEntity
    {
		public int CharacterId { get; set; }
		public string UserId { get; set; }
		public IdentityUser User { get; set; }
		public string BlogShortname { get; set; }
		public bool IsOnHiatus { get; set; }
    }
}
