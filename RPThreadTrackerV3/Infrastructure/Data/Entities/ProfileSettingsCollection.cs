// <copyright file="ProfileSettingsCollection.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using System;
	using System.ComponentModel.DataAnnotations.Schema;
	using Interfaces.Data;
    public class ProfileSettingsCollection : IEntity
    {
		[Column("SettingsId")]
		public int ProfileSettingsCollectionId { get; set; }
		public string UserId { get; set; }
		public bool ShowDashboardThreadDistribution { get; set; }
		public bool UseInvertedTheme { get; set; }
		public bool AllowMarkQueued { get; set; }
		public DateTime? LastNewsReadDate { get; set; }

	}
}
