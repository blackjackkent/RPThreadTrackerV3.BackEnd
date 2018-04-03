namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Threading.Tasks;
	using Interfaces.Services;
	using Microsoft.Extensions.Configuration;
	using Models.ViewModels;
	using SendGrid;
	using SendGrid.Helpers.Mail;

	public class SendGridEmailClient : IEmailClient
	{
		public async Task SendEmail(EmailDto email, IConfiguration config)
		{
			var apiKey = config["SendGridAPIKey"];
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress(config["ForgotPasswordEmailFromAddress"], "RPThreadTracker");
			var subject = "RPThreadTracker Password Reset";
			var to = new EmailAddress(email.RecipientEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, email.PlainTextBody, email.Body);
			var response = await client.SendEmailAsync(msg);
		}
	}
}