// <copyright file="PublicViewUrlResolver.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Mappers.Resolvers
{
    using AutoMapper;
    using Interfaces.Services;
    using Microsoft.Extensions.Configuration;
    using Models.Configuration;
    using Models.DomainModels.PublicViews;
    using Models.ViewModels.PublicViews;

    /// <summary>
    /// AutoMapper property resolver for generating a public view's full URL based on the front-end base URL.
    /// </summary>
    /// <seealso cref="IValueResolver{TSource,TDestination,TDestMember}"/>
    public class PublicViewUrlResolver : IValueResolver<PublicView, PublicViewDto, string>
    {
        private readonly AppSettings _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicViewUrlResolver"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public PublicViewUrlResolver(AppSettings config)
        {
            _config = config;
        }

        /// <summary>
        /// Uses source public view and application config to generate a full URL for the public view.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object, if exists</param>
        /// <param name="destMember">Destination member</param>
        /// <param name="context">The context of the mapping</param>
        /// <returns>
        /// A full URL for the destination public view.
        /// </returns>
        public string Resolve(PublicView source, PublicViewDto destination, string destMember, ResolutionContext context)
        {
            var baseUrl = _config.Cors.CorsUrl;
            return baseUrl + "/public/" + source.Slug;
        }
    }
}
