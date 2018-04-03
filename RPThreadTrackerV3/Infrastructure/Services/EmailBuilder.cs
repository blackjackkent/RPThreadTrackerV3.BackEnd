﻿namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Net;
	using System.Text;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Identity;
	using Models.ViewModels;

	public class EmailBuilder : IEmailBuilder
    {
	    public EmailDto BuildForgotPasswordEmail(IdentityUser user, string urlRoot, string code)
	    {
		    var resetPasswordUrl = GetResetPasswordLink(urlRoot, user.Email, code);
		    var result = new EmailDto
		    {
			    RecipientEmail = user.Email,
			    Subject = "RPThreadTracker Password Reset",
				Body = GetForgotPasswordHtmlBody(resetPasswordUrl),
				PlainTextBody = GetForgotPasswordPlainTextBody(resetPasswordUrl)
		    };
		    return result;
	    }

	    private string GetForgotPasswordPlainTextBody(string resetPasswordUrl)
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

	    private string GetForgotPasswordHtmlBody(string resetPasswordUrl)
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

	    private string GetResetPasswordLink(string urlRoot, string userEmail, string code)
	    {
		    return $"{urlRoot}/resetpassword?email={WebUtility.UrlEncode(userEmail)}&code={WebUtility.UrlEncode(code)}";
	    }
    }
}

