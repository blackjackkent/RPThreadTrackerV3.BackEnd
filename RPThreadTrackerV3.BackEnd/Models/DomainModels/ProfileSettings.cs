// <copyright file="ProfileSettings.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.DomainModels
{
    using System;

    /// <summary>
    /// Domain-layer representation of profile settings associated with a user.
    /// </summary>
	public class ProfileSettings
    {
        /// <summary>
        /// Gets or sets the unique ID of this profile settings collection.
        /// </summary>
        /// <value>
        /// The unique ID of this profile settings collection.
        /// </value>
	    public int SettingsId { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the user with whom these settings are associated.
        /// </summary>
        /// <value>
        /// The unique ID of the user with whom these settings are associated.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user wishes to display the dashboard "At a Glance" witdget.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the user wishes to display the dashboard "At a Glance" witdget; otherwise, <c>false</c>.
        /// </value>
        public bool ShowDashboardThreadDistribution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user wishes to use an inverted CSS theme.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the user wishes to use an inverted CSS theme; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("No longer relevant after 2018 redesign.")]
        public bool UseInvertedTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has opted-in to the "Mark Queued" feature.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the user has opted-in to the "Mark Queued" feature; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("No longer relevant after 2018 redesign.")]
        public bool AllowMarkQueued { get; set; }

        /// <summary>
        /// Gets or sets the last datetime at which the user viewed the news widget.
        /// </summary>
        /// <value>
        /// The last datetime at which the user viewed the news widget.
        /// </value>
        public DateTime? LastNewsReadDate { get; set; }
	}
}
