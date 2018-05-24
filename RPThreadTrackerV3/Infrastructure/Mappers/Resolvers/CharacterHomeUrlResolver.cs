// <copyright file="CharacterHomeUrlResolver.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Mappers.Resolvers
{
	using AutoMapper;
	using Enums;
	using Models.DomainModels;
	using Models.ViewModels;

    /// <summary>
    /// AutoMapper property resolver for generating a character's home URL based on its hosting platform.
    /// </summary>
    /// <seealso cref="IValueResolver{TSource,TDestination,TDestMember}"/>
	public class CharacterHomeUrlResolver : IValueResolver<Character, CharacterDto, string>
	{
        /// <summary>
        /// Uses source character to generate a home URL for the destination character based on its hosting platform.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object, if exists</param>
        /// <param name="destMember">Destination member</param>
        /// <param name="context">The context of the mapping</param>
        /// <returns>
        /// Home URL for the destination character based on its hosting platform.
        /// </returns>
        public string Resolve(Character source, CharacterDto destination, string destMember, ResolutionContext context)
		{
			switch (source.PlatformId)
			{
				case Platform.Tumblr:
					return GetTumblrHomeUrl(source);
				default:
					return null;
			}
		}

		private static string GetTumblrHomeUrl(Character source)
		{
			return $"http://{source.UrlIdentifier}.tumblr.com";
		}
	}
}
