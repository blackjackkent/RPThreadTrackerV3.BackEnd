// <copyright file="UserMapper.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Identity;
	using Models.DomainModels;
	using Models.ViewModels;

	public class UserMapper : Profile
	{
		public UserMapper()
		{
			CreateMap<User, IdentityUser>()
				.ReverseMap();
			CreateMap<User, UserDto>()
				.ReverseMap();
		}
	}
}
