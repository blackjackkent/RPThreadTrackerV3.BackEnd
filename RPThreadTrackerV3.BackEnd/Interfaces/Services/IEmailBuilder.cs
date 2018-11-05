// <copyright file="IEmailBuilder.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using Microsoft.AspNetCore.Identity;
    using Models.Configuration;
    using Models.ViewModels;

    /// <summary>
    /// Builder which generates emails of predefined types.
    /// </summary>
    public interface IEmailBuilder
	{
	    /// <summary>
	    /// Builds the forgot password email.
	    /// </summary>
	    /// <param name="user">The user to whom the email should be sent.</param>
	    /// <param name="urlRoot">The URL root of the front-end site.</param>
	    /// <param name="code">The password reset code.</param>
	    /// <param name="config">The app configuration.</param>
	    /// <returns><see cref="EmailDto"/> object containing information about the message to be sent.</returns>
	    EmailDto BuildForgotPasswordEmail(IdentityUser user, string urlRoot, string code, AppSettings config);

        /// <summary>
        /// Builds the "Contact Us" email.
        /// </summary>
        /// <param name="email">The admin email to which the message should be sent.</param>
        /// <param name="username">Username of the user sending the message.</param>
        /// <param name="message">The message.</param>
        /// <param name="config">The configuration.</param>
        /// <returns><see cref="EmailDto"/> object containing information about the message to be sent.</returns>
        EmailDto BuildContactEmail(string email, string username, string message, AppSettings config);
	}
}
