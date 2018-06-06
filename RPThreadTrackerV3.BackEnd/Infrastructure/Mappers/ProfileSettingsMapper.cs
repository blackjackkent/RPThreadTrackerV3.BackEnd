// <copyright file="ProfileSettingsMapper.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Mappers
{
    using AutoMapper;
    using Data.Entities;
    using Models.DomainModels;
    using Models.ViewModels;

    /// <summary>
    /// Mapping class for mapping between view model, domain model, and entity representations of user profile settings.
    /// </summary>
    /// <seealso cref="Profile" />
	public class ProfileSettingsMapper : Profile
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSettingsMapper"/> class.
        /// </summary>
        public ProfileSettingsMapper()
		{
			CreateMap<ProfileSettings, ProfileSettingsCollection>()
				.ForMember(d => d.ProfileSettingsCollectionId, o => o.MapFrom(s => s.SettingsId))
				.ReverseMap()
				.ForMember(d => d.SettingsId, o => o.MapFrom(s => s.ProfileSettingsCollectionId));
			CreateMap<ProfileSettings, ProfileSettingsDto>()
				.ReverseMap();
		}
	}
}
