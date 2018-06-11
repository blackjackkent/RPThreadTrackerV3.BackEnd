// <copyright file="AppSettings.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.Configuration
{
    /// <summary>
    /// Wrapper class for application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the application settings related to auth tokens.
        /// </summary>
        /// <value>
        /// The application settings related to auth tokens.
        /// </value>
        public TokensAppSettings Tokens { get; set; }

        /// <summary>
        /// Gets or sets the cors.
        /// </summary>
        /// <value>
        /// The cors.
        /// </value>
        public CorsAppSettings Cors { get; set; }

        public MaintenanceAppSettings Maintenance { get; set; }

        public SecureAppSettings Secure { get; set; }
    }
}
