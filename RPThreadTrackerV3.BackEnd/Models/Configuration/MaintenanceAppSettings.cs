// <copyright file="MaintenanceAppSettings.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.Configuration
{
    /// <summary>
    /// Wrapper class for application settings related to site maintenance.
    /// </summary>
    public class MaintenanceAppSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the site is in maintenance mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the site is in maintenance mode; otherwise, <c>false</c>.
        /// </value>
        public bool MaintenanceMode { get; set; }
    }
}
