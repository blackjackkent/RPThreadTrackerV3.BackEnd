// <copyright file="IEmailClient.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using System.Threading.Tasks;
    using Models.Configuration;
    using Models.ViewModels;

    /// <summary>
    /// Client for sending emails.
    /// </summary>
    public interface IEmailClient
	{
	    /// <summary>
	    /// Sends the email.
	    /// </summary>
	    /// <param name="email">The email.</param>
	    /// <param name="config">The app configuration.</param>
	    /// <returns>
	    /// A task that represents the asynchronous operation.
	    /// </returns>
	    Task SendEmail(EmailDto email, AppSettings config);
	}
}
