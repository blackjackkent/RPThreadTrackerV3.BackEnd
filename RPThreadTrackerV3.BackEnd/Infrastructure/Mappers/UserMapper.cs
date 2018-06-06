// <copyright file="UserMapper.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Mappers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Models.DomainModels;
    using Models.ViewModels;

    /// <summary>
    /// Mapping class for mapping between view model, domain model, and entity representations of characters.
    /// </summary>
    /// <seealso cref="Profile" />
	public class UserMapper : Profile
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMapper"/> class.
        /// </summary>
        public UserMapper()
		{
			CreateMap<User, IdentityUser>()
				.ReverseMap();
			CreateMap<User, UserDto>()
				.ReverseMap();
		}
	}
}
