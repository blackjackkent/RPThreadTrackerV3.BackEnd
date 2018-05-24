// <copyright file="Character.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using Enums;
	using Interfaces.Data;
	using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Data-layer representation of a character written about in various threads.
    /// </summary>
    /// <seealso cref="IEntity" />
    public class Character : IEntity
    {
        /// <summary>
        /// Gets or sets the unique ID of this character.
        /// </summary>
        /// <value>
        /// The unique ID of this character.
        /// </value>
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the user who owns this character.
        /// </summary>
        /// <value>
        /// The unique ID of the user who owns this character.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user who owns this character.
        /// </summary>
        /// <value>
        /// The user who owns this character.
        /// </value>
        public IdentityUser User { get; set; }

        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        /// <value>
        /// The name of the character.
        /// </value>
        public string CharacterName { get; set; }

        /// <summary>
        /// Gets or sets the URL identifier of the character on their hosting platform.
        /// </summary>
        /// <value>
        /// The URL identifier of the character on their hosting platform.
        /// </value>
        public string UrlIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this character is marked as on hiatus.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this character is marked as on hiatus; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnHiatus { get; set; }

        /// <summary>
        /// Gets or sets the ID of the hosting platform with which this character is associated.
        /// </summary>
        /// <value>
        /// The ID of the hosting platform with which this character is associated.
        /// </value>
        public Platform PlatformId { get; set; }
    }
}
