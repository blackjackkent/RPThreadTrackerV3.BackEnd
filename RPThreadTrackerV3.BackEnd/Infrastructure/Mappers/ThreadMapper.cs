// <copyright file="ThreadMapper.cs" company="Rosalind Wills">
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
    /// Mapping class for mapping between view model, domain model, and entity representations of threads.
    /// </summary>
    /// <seealso cref="Profile" />
    [ExcludeFromCodeCoverage]
	public class ThreadMapper : Profile
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadMapper"/> class.
        /// </summary>
        public ThreadMapper()
		{
			CreateMap<Thread, Data.Entities.Thread>()
				.ReverseMap();
			CreateMap<Thread, ThreadDto>()
				.ForMember(d => d.ThreadHomeUrl, o => o.MapFrom<ThreadHomeUrlResolver>())
				.ReverseMap();
			CreateMap<ThreadTag, Data.Entities.ThreadTag>()
				.ReverseMap();
			CreateMap<ThreadTag, ThreadTagDto>()
				.ReverseMap();
		}
	}
}
