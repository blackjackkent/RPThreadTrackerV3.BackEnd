namespace RPThreadTrackerV3.Infrastructure.Data.Seeders
{
	using System.Threading.Tasks;
	using Enums;
	using Microsoft.AspNetCore.Identity;

	public class RoleInitializer
	{
		private readonly RoleManager<IdentityRole> _roleManager;

		public RoleInitializer(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task Seed()
		{
			await InitRoles();
		}

		private async Task InitRoles()
		{
			var userRole = await _roleManager.FindByNameAsync(RoleConstants.USER_ROLE);
			if (userRole == null)
			{
				userRole = new IdentityRole(RoleConstants.USER_ROLE);
				await _roleManager.CreateAsync(userRole);
			}
		}
	}
}
