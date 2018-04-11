namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using AutoMapper;
	using Data;
	using Infrastructure.Data.Entities;
	using Microsoft.AspNetCore.Identity;
	using Models.DomainModels;
	using Models.RequestModels;
	using Models.ViewModels;

	public interface IAuthService
    {
	    Task<IdentityUser> GetUserByUsernameOrEmail(string modelUsername, UserManager<IdentityUser> userManager);
	    Task<JwtSecurityToken> GenerateJwt(IdentityUser user, string key, string issuer, string audience, UserManager<IdentityUser> userManager);
	    Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper);
	    ProfileSettings GetProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper);
	    void UpdateProfileSettings(ProfileSettings settings, string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper);
	    void InitProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository);
	    Task ResetPassword(string email, string passwordResetToken, string newPassword, UserManager<IdentityUser> userManager);
	    Task ChangePassword(ClaimsPrincipal user, string currentPassword, string newPassword, string confirmNewPassword, UserManager<IdentityUser> userManager);
	    Task ChangeAccountInformation(ClaimsPrincipal user, string email, string username, UserManager<IdentityUser> userManager);
    }
}
