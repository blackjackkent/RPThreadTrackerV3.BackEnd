namespace RPThreadTrackerV3.Interfaces.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Models.ViewModels;

	public interface IEmailBuilder
	{
		EmailDto BuildForgotPasswordEmail(IdentityUser user, string urlRoot, string code, IConfiguration config);
	    EmailDto BuildContactEmail(string email, string userName, string message, IConfiguration config);
	}
}
