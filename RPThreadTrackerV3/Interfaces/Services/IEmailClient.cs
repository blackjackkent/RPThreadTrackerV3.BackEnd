namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Configuration;
	using Models.ViewModels;

	public interface IEmailClient
	{
		Task SendEmail(EmailDto email, IConfiguration config);
	}
}
