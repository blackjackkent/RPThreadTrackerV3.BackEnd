// <copyright file="PublicViewMapper.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Mappers
{
    using System.Diagnostics.CodeAnalysis;
    using AutoMapper;
    using Models.DomainModels.PublicViews;
    using Models.ViewModels.PublicViews;
    using Resolvers;

    /// <summary>
    /// Mapping class for mapping between view model, domain model, and entity representations of public views.
    /// </summary>
    /// <seealso cref="Profile" />
    [ExcludeFromCodeCoverage]
    public class PublicViewMapper : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicViewMapper"/> class.
        /// </summary>
        public PublicViewMapper()
        {
            CreateMap<PublicView, Data.Documents.PublicView>()
                .ReverseMap();
            CreateMap<PublicView, PublicViewDto>()
                .ReverseMap();
            CreateMap<PublicTurnFilter, Data.Documents.PublicTurnFilter>()
                .ReverseMap();
            CreateMap<PublicTurnFilter, PublicTurnFilterDto>()
                .ReverseMap();
        }
    }
}
