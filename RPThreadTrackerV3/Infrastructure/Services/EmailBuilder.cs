﻿// <copyright file="EmailBuilder.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Services
{
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Models.ViewModels;

    /// <inheritdoc />
    public class EmailBuilder : IEmailBuilder
    {
        /// <inheritdoc />
        public EmailDto BuildForgotPasswordEmail(IdentityUser user, string urlRoot, string code, IConfiguration config)
	    {
		    var resetPasswordUrl = GetResetPasswordLink(urlRoot, user.Email, code);
		    var result = new EmailDto
		    {
			    RecipientEmail = user.Email,
			    Subject = "RPThreadTracker Password Reset",
				Body = GetForgotPasswordHtmlBody(resetPasswordUrl),
				PlainTextBody = GetForgotPasswordPlainTextBody(resetPasswordUrl),
                SenderEmail = config["ForgotPasswordEmailFromAddress"],
                SenderName = "RPThreadTracker"
            };
		    return result;
	    }

        /// <inheritdoc />
        public EmailDto BuildContactEmail(string userEmail, string username, string modelMessage, IConfiguration config)
        {
            return new EmailDto
            {
                RecipientEmail = config["ContactFormEmailToAddress"],
                Body = "<p>You have received a message via RPThreadTracker's contact form:</p>" +
                       Regex.Replace(modelMessage, @"\r\n?|\n", "<br />"),
                PlainTextBody = "You have received a message via RPThreadTracker's contact form:\n" + modelMessage,
                SenderName = username,
                SenderEmail = userEmail,
                Subject = "RPThreadTracker Contact Form Submission"
            };
        }

        private static string GetForgotPasswordPlainTextBody(string resetPasswordUrl)
		{
			var bodyBuilder = new StringBuilder();
			bodyBuilder.Append("Hello,\n\n");
			bodyBuilder.Append("You are receiving this message because you requested to reset your password for rpthreadtracker.com.\n\n");
			bodyBuilder.Append("Please use the link below to perform a password reset.\n\n");
			bodyBuilder.Append($"{resetPasswordUrl}\n\n");
			bodyBuilder.Append("Thanks, and have a great day!\n\n");
			bodyBuilder.Append("~Tracker-mun");
			return bodyBuilder.ToString();
		}

	    private static string GetForgotPasswordHtmlBody(string resetPasswordUrl)
	    {
		    var bodyBuilder = new StringBuilder();
		    bodyBuilder.Append("<p>Hello,</p>");
		    bodyBuilder.Append("<p>You are receiving this message because you requested to reset your password for <a href=\"http://www.rpthreadtracker.com\">rpthreadtracker.com</a>.</p>");
		    bodyBuilder.Append("<p>Please use the link below to perform a password reset.</p>");
		    bodyBuilder.Append($"<p>{resetPasswordUrl}</p>");
			bodyBuilder.Append("<p>Thanks, and have a great day!</p>");
		    bodyBuilder.Append("<p>~Tracker-mun</p>");
		    return bodyBuilder.ToString();
		}

	    private static string GetResetPasswordLink(string urlRoot, string userEmail, string code)
	    {
		    return $"{urlRoot}/resetpassword?email={WebUtility.UrlEncode(userEmail)}&code={WebUtility.UrlEncode(code)}";
	    }
    }
}
