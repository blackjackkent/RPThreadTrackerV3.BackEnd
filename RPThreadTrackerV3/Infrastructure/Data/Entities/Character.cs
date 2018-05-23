﻿// <copyright file="Character.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using Enums;
	using Interfaces.Data;
	using Microsoft.AspNetCore.Identity;

	public class Character : IEntity
    {
		public int CharacterId { get; set; }
		public string UserId { get; set; }
		public IdentityUser User { get; set; }
		public string CharacterName { get; set; }
		public string UrlIdentifier { get; set; }
		public bool IsOnHiatus { get; set; }
		public Platform PlatformId { get; set; }
    }
}
