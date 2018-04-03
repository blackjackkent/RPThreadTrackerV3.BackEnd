namespace RPThreadTrackerV3.Interfaces.Services
{
	using Microsoft.AspNetCore.Identity;
	using Models.ViewModels;

	public interface IEmailBuilder
	{
		EmailDto BuildForgotPasswordEmail(IdentityUser user, string urlRoot, string code);
	}
}
