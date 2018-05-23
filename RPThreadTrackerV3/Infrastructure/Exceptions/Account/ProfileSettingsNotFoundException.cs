// <copyright file="ProfileSettingsNotFoundException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;

    public class ProfileSettingsNotFoundException : Exception
    {
		public ProfileSettingsNotFoundException()
		    : base("No profile settings exist for the current user.") { }
    }
}
