// <copyright file="CorsAppSettings.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.Configuration
{
    /// <summary>
    /// Wrapper class for application settings related to cross-origin requests.
    /// </summary>
    public class CorsAppSettings
    {
        /// <summary>
        /// Gets or sets the base URL allowed for CORS requests.
        /// </summary>
        /// <value>
        /// The base URL allowed for CORS requests.
        /// </value>
        public string CorsUrl { get; set; }
    }
}
