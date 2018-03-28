namespace RPThreadTrackerV3.Infrastructure.Data
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Entities;

	public class TrackerContext : IdentityDbContext
	{
		public TrackerContext(DbContextOptions options)
			: base(options)
		{ }

		public DbSet<IdentityUser> Users { get; set; }
		public DbSet<Thread> Threads { get; set; }
		public DbSet<Character> Characters { get; set; }
		public DbSet<ProfileSettingsCollection> ProfileSettings { get; set; }
		public DbSet<ThreadTag> ThreadTags { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
	}
}
