// <copyright file="ProfileSettingsNotFoundException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error retrieving a user's profile settings.
    /// </summary>
    /// <seealso cref="Exception" />
    public class ProfileSettingsNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSettingsNotFoundException"/> class.
        /// </summary>
        public ProfileSettingsNotFoundException()
            : base("No profile settings exist for the current user.")
        {
        }
    }
}
