// <copyright file="ConfigurationService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Services
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Interfaces.Services;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class ConfigurationService : IConfigurationService
    {
        /// <inheritdoc />
        public string CorsUrl => ConfigurationManager.AppSettings["CorsUrl"];

        /// <inheritdoc />
        public bool MaintenanceMode
        {
            get
            {
                var stringValue = ConfigurationManager.AppSettings["Tokens:RefreshExpireMinutes"];
                return bool.Parse(stringValue);
            }
        }

        #region Emails

        /// <inheritdoc />
        public string SendGridApiKey => ConfigurationManager.AppSettings["SendGridAPIKey"];

        /// <inheritdoc />
        public string ForgotPasswordEmailFromAddress => ConfigurationManager.AppSettings["ForgotPasswordEmailFromAddress"];

        /// <inheritdoc />
        public string ContactFormEmailToAddress => ConfigurationManager.AppSettings["ContactFormEmailToAddress"];

        #endregion

        #region DocumentDB

        /// <inheritdoc />
        public string DocumentsKey => ConfigurationManager.AppSettings["Documents:Key"];

        /// <inheritdoc />
        public string DocumentsEndpoint => ConfigurationManager.AppSettings["Documents:Endpoint"];

        /// <inheritdoc />
        public string DocumentsDatabaseId => ConfigurationManager.AppSettings["Documents:DatabaseId"];

        /// <inheritdoc />
        public string DocumentsCollectionId => ConfigurationManager.AppSettings["Documents:CollectionId"];

        #endregion

        #region JWT Tokens

        /// <inheritdoc />
        public string JwtTokenKey => ConfigurationManager.AppSettings["Tokens:Key"];

        /// <inheritdoc />
        public string JwtTokenIssuer => ConfigurationManager.AppSettings["Tokens:Issuer"];

        /// <inheritdoc />
        public string JwtTokenAudience => ConfigurationManager.AppSettings["Tokens:Audience"];

        /// <inheritdoc />
        public double JwtAccessTokenExpirationMinutes
        {
            get
            {
                var stringValue = ConfigurationManager.AppSettings["Tokens:AccessExpireMinutes"];
                return double.Parse(stringValue, CultureInfo.CurrentCulture);
            }
        }

        /// <inheritdoc />
        public double JwtRefreshTokenExpirationMinutes
        {
            get
            {
                var stringValue = ConfigurationManager.AppSettings["Tokens:RefreshExpireMinutes"];
                return double.Parse(stringValue, CultureInfo.CurrentCulture);
            }
        }

        #endregion
    }
}
