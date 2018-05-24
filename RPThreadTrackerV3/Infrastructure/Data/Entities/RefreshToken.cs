// <copyright file="RefreshToken.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
    using System;
    using Microsoft.AspNetCore.Identity;
    using RPThreadTrackerV3.Interfaces.Data;

    /// <summary>
    /// Data-layer representation of refresh token information used to maintain
    /// a user's session.
    /// </summary>
    /// <seealso cref="IEntity" />
    public class RefreshToken : IEntity
	{
        /// <summary>
        /// Gets or sets the unique identifier of this refresh token.
        /// </summary>
        /// <value>
        /// The unique identifier of this refresh token.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the datetime at which this refresh token was issued.
        /// </summary>
        /// <value>
        /// The datetime at which this refresh token was issued.
        /// </value>
        public DateTime IssuedUtc { get; set; }

	    /// <summary>
	    /// Gets or sets the datetime at which this refresh token will expire.
	    /// </summary>
	    /// <value>
	    /// The datetime at which this refresh token will expire.
	    /// </value>
        public DateTime ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets the refresh token value.
        /// </summary>
        /// <value>
        /// The refresh token value.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the user this token is associated with.
        /// </summary>
        /// <value>
        /// Th unique ID of the user this token is associated with.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user this token is associated with.
        /// </summary>
        /// <value>
        /// The user this token is associated with.
        /// </value>
        public IdentityUser User { get; set; }
	}
}
