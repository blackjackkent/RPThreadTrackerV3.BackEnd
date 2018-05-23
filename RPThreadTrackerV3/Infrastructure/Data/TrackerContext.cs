namespace RPThreadTrackerV3.Infrastructure.Data
{
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

	public class TrackerContext : IdentityDbContext
	{
	    public TrackerContext(DbContextOptions options)
	        : base(options)
	    {
	    }

		public DbSet<Thread> Threads { get; set; }
		public DbSet<Character> Characters { get; set; }
		public DbSet<ProfileSettingsCollection> ProfileSettings { get; set; }
		public DbSet<ThreadTag> ThreadTags { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
	}
}
