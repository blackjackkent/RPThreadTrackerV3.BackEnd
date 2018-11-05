// <copyright file="CharacterMapper.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Mappers
{
    using System.Diagnostics.CodeAnalysis;
    using AutoMapper;
    using Models.DomainModels;
    using Models.ViewModels;
    using Resolvers;

    /// <summary>
    /// Mapping class for mapping between view model, domain model, and entity representations of characters.
    /// </summary>
    /// <seealso cref="Profile" />
    [ExcludeFromCodeCoverage]
    public class CharacterMapper : Profile
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapper"/> class.
        /// </summary>
        public CharacterMapper()
		{
			CreateMap<Character, Data.Entities.Character>()
				.ForMember(d => d.User, o => o.Ignore())
				.ReverseMap();
			CreateMap<Character, CharacterDto>()
				.ForMember(d => d.HomeUrl, o => o.ResolveUsing<CharacterHomeUrlResolver>())
				.ReverseMap();
		}
	}
}
