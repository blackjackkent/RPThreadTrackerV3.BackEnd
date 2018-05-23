// <copyright file="InvalidThreadException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Thread
{
    using System;

    public class InvalidThreadException : Exception
    {
		public InvalidThreadException()
		    : base("The supplied thread was invalid.") { }
    }
}
