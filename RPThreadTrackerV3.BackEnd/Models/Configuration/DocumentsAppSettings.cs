// <copyright file="DocumentsAppSettings.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.Configuration
{
    /// <summary>
    /// Wrapper class for app settings related to document storage.
    /// </summary>
    public class DocumentsAppSettings
    {
        /// <summary>
        /// Gets or sets the document database key.
        /// </summary>
        /// <value>
        /// The document database key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the document database endpoint.
        /// </summary>
        /// <value>
        /// The document database endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the database database ID.
        /// </summary>
        /// <value>
        /// The document database ID.
        /// </value>
        public string DatabaseId { get; set; }

        /// <summary>
        /// Gets or sets the document database collection ID.
        /// </summary>
        /// <value>
        /// The document database collection ID.
        /// </value>
        public string CollectionId { get; set; }
    }
}
