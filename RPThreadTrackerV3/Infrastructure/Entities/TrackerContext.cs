using Microsoft.AspNetCore.Identity;

namespace RPThreadTrackerV3.Infrastructure.Entities
{
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	public class TrackerContext : IdentityDbContext
	{
		public TrackerContext(DbContextOptions options)
			: base(options)
		{ }

		public DbSet<IdentityUser> Users { get; set; }
	}
}
