// <copyright file="IExporterService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using Models.DomainModels;

	public interface IExporterService
    {
	    byte[] GetByteArray(IEnumerable<Character> characters, Dictionary<int, List<Thread>> threads);
    }
}
