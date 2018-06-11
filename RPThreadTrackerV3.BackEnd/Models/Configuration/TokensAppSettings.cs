// <copyright file="TokensAppSettings.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.Configuration
{
    /// <summary>
    /// Wrapper class for application settings related to auth tokens.
    /// </summary>
    public class TokensAppSettings
    {
        /// <summary>
        /// Gets or sets the auth token key.
        /// </summary>
        /// <value>
        /// The auth token key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the auth token issuer.
        /// </summary>
        /// <value>
        /// The auth token issuer.
        /// </value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the auth token audience.
        /// </summary>
        /// <value>
        /// The auth token audience.
        /// </value>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the access token duration in minutes.
        /// </summary>
        /// <value>
        /// The access token duration in minutes.
        /// </value>
        public int AccessExpireMinutes { get; set; }

        /// <summary>
        /// Gets or sets the refresh token duration in minutes.
        /// </summary>
        /// <value>
        /// The refresh token duration in minutes.
        /// </value>
        public int RefreshExpireMinutes { get; set; }
    }
}
