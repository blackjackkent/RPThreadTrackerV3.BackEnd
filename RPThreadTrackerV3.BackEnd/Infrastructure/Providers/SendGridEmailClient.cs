// <copyright file="SendGridEmailClient.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Providers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Interfaces.Services;
    using Microsoft.Extensions.Configuration;
    using Models.ViewModels;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    /// <summary>
    /// Client for sending emails using SendGrid
    /// </summary>
    /// <seealso cref="IEmailClient" />
    [ExcludeFromCodeCoverage]
    public class SendGridEmailClient : IEmailClient
	{
	    private readonly SendGridClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridEmailClient"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public SendGridEmailClient(IConfiguration config)
	    {
	        var apiKey = config["SendGridAPIKey"];
            _client = new SendGridClient(apiKey);
	    }

	    /// <inheritdoc />
	    public async Task SendEmail(EmailDto email, IConfiguration config)
		{
            var from = new EmailAddress(email.SenderEmail, email.SenderName);
            var to = new EmailAddress(email.RecipientEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextBody, email.Body);
			await _client.SendEmailAsync(msg);
		}
	}
}
