// <copyright file="AuthTokenCollection.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.ViewModels.Auth
{
    /// <summary>
    /// Collection combining an access token and refresh token for use by a front-end application in authenticating with this API.
    /// </summary>
    public class AuthTokenCollection
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public AuthToken Token { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public AuthToken RefreshToken { get; set; }
    }
}
