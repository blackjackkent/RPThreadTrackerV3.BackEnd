// <copyright file="IConfigurationService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    /// <summary>
    /// Wrapper service for configuration values.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets the base CORS URL.
        /// </summary>
        /// <value>
        /// The base CORS URL.
        /// </value>
        string CorsUrl { get; }

        /// <summary>
        /// Gets the document database key.
        /// </summary>
        /// <value>
        /// The document database key.
        /// </value>
        string DocumentsKey { get; }

        /// <summary>
        /// Gets the document database endpoint.
        /// </summary>
        /// <value>
        /// The document database endpoint.
        /// </value>
        string DocumentsEndpoint { get; }

        /// <summary>
        /// Gets the document database ID.
        /// </summary>
        /// <value>
        /// The document database ID.
        /// </value>
        string DocumentsDatabaseId { get; }

        /// <summary>
        /// Gets the document database collection identifier.
        /// </summary>
        /// <value>
        /// The document database collection identifier.
        /// </value>
        string DocumentsCollectionId { get; }

        /// <summary>
        /// Gets the SendGrid API key for sending emails.
        /// </summary>
        /// <value>
        /// The SendGrid API key.
        /// </value>
        string SendGridApiKey { get; }

        /// <summary>
        /// Gets the JWT token key.
        /// </summary>
        /// <value>
        /// The JWT token key.
        /// </value>
        string JwtTokenKey { get; }

        /// <summary>
        /// Gets the JWT token issuer.
        /// </summary>
        /// <value>
        /// The JWT token issuer.
        /// </value>
        string JwtTokenIssuer { get; }

        /// <summary>
        /// Gets the JWT token audience.
        /// </summary>
        /// <value>
        /// The JWT token audience.
        /// </value>
        string JwtTokenAudience { get; }

        /// <summary>
        /// Gets the JWT access token expiration time in minutes.
        /// </summary>
        /// <value>
        /// The JWT access token expiration time in minutes.
        /// </value>
        double JwtAccessTokenExpirationMinutes { get; }

        /// <summary>
        /// Gets the JWT refresh token expiration time in minutes.
        /// </summary>
        /// <value>
        /// The JWT refresh token expiration time in minutes.
        /// </value>
        double JwtRefreshTokenExpirationMinutes { get; }

        /// <summary>
        /// Gets the email address which should show in the "From" field of Forgot Password emails.
        /// </summary>
        /// <value>
        /// The "From" email address for Forgot Password emails.
        /// </value>
        string ForgotPasswordEmailFromAddress { get; }

        /// <summary>
        /// Gets the email address to which contact form emails should be sent.
        /// </summary>
        /// <value>
        /// The email address to which contact form emails should be sent.
        /// </value>
        string ContactFormEmailToAddress { get; }

        /// <summary>
        /// Gets a value indicating whether the site is in maintenance mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the site is in maintenance mode; otherwise, <c>false</c>.
        /// </value>
        bool MaintenanceMode { get; }
    }
}
