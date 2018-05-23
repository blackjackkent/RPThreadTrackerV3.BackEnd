// <copyright file="RefreshToken.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
    using System;
    using Microsoft.AspNetCore.Identity;
    using RPThreadTrackerV3.Interfaces.Data;

	public class RefreshToken : IEntity
	{
	    public string Id { get; set; }
	    public DateTime IssuedUtc { get; set; }
	    public DateTime ExpiresUtc { get; set; }
	    public string Token { get; set; }

	    public string UserId { get; set; }
	    public IdentityUser User { get; set; }
	}
}
