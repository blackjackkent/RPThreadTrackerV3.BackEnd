// <copyright file="TrackerContext.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data
{
    using Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database context for entity management.
    /// </summary>
    /// <seealso cref="IdentityDbContext" />
    public class TrackerContext : IdentityDbContext
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext" />.</param>
        public TrackerContext(DbContextOptions options)
	        : base(options)
	    {
	    }

        /// <summary>
        /// Gets or sets the dataset containing thread entities.
        /// </summary>
        /// <value>
        /// The dataset containing thread entities.
        /// </value>
        public DbSet<Thread> Threads { get; set; }

	    /// <summary>
	    /// Gets or sets the dataset containing character entities.
	    /// </summary>
	    /// <value>
	    /// The dataset containing character entities.
	    /// </value>
        public DbSet<Character> Characters { get; set; }

	    /// <summary>
	    /// Gets or sets the dataset containing user profile settings entities.
	    /// </summary>
	    /// <value>
	    /// The dataset containing user profile settings entities.
	    /// </value>
        public DbSet<ProfileSettingsCollection> ProfileSettings { get; set; }

	    /// <summary>
	    /// Gets or sets the dataset containing thread tag entities.
	    /// </summary>
	    /// <value>
	    /// The dataset containing thread tag entities.
	    /// </value>
        public DbSet<ThreadTag> ThreadTags { get; set; }

	    /// <summary>
	    /// Gets or sets the dataset containing refresh token entities.
	    /// </summary>
	    /// <value>
	    /// The dataset containing thread entities.
	    /// </value>
        public DbSet<RefreshToken> RefreshTokens { get; set; }
	}
}
