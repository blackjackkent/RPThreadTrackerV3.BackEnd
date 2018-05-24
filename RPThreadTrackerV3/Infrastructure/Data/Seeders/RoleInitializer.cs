// <copyright file="RoleInitializer.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Seeders
{
	using System.Threading.Tasks;
	using Enums;
	using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Seeder class used to initialize missing user roles at application start.
    /// </summary>
    public class RoleInitializer
	{
		private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleInitializer"/> class.
        /// </summary>
        /// <param name="roleManager">The role manager.</param>
        public RoleInitializer(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

        /// <summary>
        /// Seeds the database with missing user roles.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
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
