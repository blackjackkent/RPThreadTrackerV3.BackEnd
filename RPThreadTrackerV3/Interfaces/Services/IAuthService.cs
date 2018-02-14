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
	using Models.ViewModels;

	public interface IAuthService
    {
	    Task<IdentityUser> GetUserByUsernameOrEmail(string modelUsername, UserManager<IdentityUser> userManager);
	    Task<JwtSecurityToken> GenerateJwt(IdentityUser user, string key, string issuer, string audience, UserManager<IdentityUser> userManager);
	    Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper);
	    ProfileSettings GetProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper, IRedisClient redisClient);
	    void UpdateProfileSettings(ProfileSettings settings, string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper, IRedisClient redisClient);
	    void InitProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository);
    }
}
