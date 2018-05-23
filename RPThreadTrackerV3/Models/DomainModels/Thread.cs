// <copyright file="Thread.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.DomainModels
{
	using System;
	using System.Collections.Generic;

	public class Thread
    {
	    public int ThreadId { get; }
	    public int CharacterId { get; }
	    public Character Character { get; }
	    public string PostId { get; }
	    public string UserTitle { get; }
	    public string PartnerUrlIdentifier { get; }
	    public bool IsArchived { get; }
	    public DateTime? DateMarkedQueued { get; }
		public List<ThreadTag> ThreadTags { get; }
	}
}
