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
            var from = new EmailAddress(email.SenderEmail, email.SenderName);
            var to = new EmailAddress(email.RecipientEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextBody, email.Body);
			await client.SendEmailAsync(msg);
		}
	}
}