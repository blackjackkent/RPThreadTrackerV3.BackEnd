// <copyright file="AuthToken.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.ViewModels.Auth
{
    using System;

    /// <summary>
    /// Class which represents a security token string and expiration date
    /// </summary>
    public class AuthToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthToken"/> class.
        /// </summary>
        public AuthToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthToken"/> class.
        /// </summary>
        /// <param name="token">The security token string</param>
        /// <param name="expiry">The DateTime at which the token will expire</param>
        public AuthToken(string token, DateTime expiry)
        {
            Token = token;
            Expiry = expiry.Ticks;
        }

        /// <summary>
        /// Gets or sets the security token.
        /// </summary>
        /// <value>
        /// String value of the security token
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the expiration DateTime of the token.
        /// </summary>
        /// <value>
        /// The expiration DateTime of the token
        /// </value>
        public long Expiry { get; set; }
    }
}
