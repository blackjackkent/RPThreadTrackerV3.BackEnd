namespace RPThreadTrackerV3.Infrastructure.Entities
{
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	public class BudgetContext : IdentityDbContext
	{
		public BudgetContext(DbContextOptions options)
			: base(options)
		{ }

		public DbSet<IdentityUser> Users { get; set; }
	}
}
