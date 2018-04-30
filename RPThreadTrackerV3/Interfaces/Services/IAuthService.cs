using System;
using Microsoft.Extensions.Configuration;
using RPThreadTrackerV3.Models.ViewModels.Auth;

namespace RPThreadTrackerV3.Interfaces.Services
{
    using System.Security.Claims;
	using System.Threading.Tasks;
	using AutoMapper;
	using Data;
	using Infrastructure.Data.Entities;
	using Microsoft.AspNetCore.Identity;
	using Models.DomainModels;

    public interface IAuthService
    {
	    Task<IdentityUser> GetUserByUsernameOrEmail(string modelUsername, UserManager<IdentityUser> userManager);
	    Task<AuthToken> GenerateJwt(IdentityUser user, UserManager<IdentityUser> userManager, IConfiguration config);
        AuthToken GenerateRefreshToken(IdentityUser userId, IConfiguration config,
            IRepository<RefreshToken> refreshTokenRepository);
        IdentityUser GetUserForRefreshToken(string refreshToken, IConfiguration config,
            IRepository<RefreshToken> refreshTokenRepository);
        Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper);
	    ProfileSettings GetProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper);
	    void UpdateProfileSettings(ProfileSettings settings, string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper);
	    void InitProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository);
	    Task ResetPassword(string email, string passwordResetToken, string newPassword, UserManager<IdentityUser> userManager);
	    Task ChangePassword(ClaimsPrincipal user, string currentPassword, string newPassword, string confirmNewPassword, UserManager<IdentityUser> userManager);
	    Task ChangeAccountInformation(ClaimsPrincipal user, string email, string username, UserManager<IdentityUser> userManager);
        Task ValidatePassword(IdentityUser user, string modelPassword, UserManager<IdentityUser> userManager);
        Task CreateUser(IdentityUser user, string modelPassword, UserManager<IdentityUser> userManager);
        Task AddUserToRole(IdentityUser user, string s, UserManager<IdentityUser> userManager);
        void RevokeRefreshToken(string modelRefreshToken, IConfiguration config, IRepository<RefreshToken> refreshTokenRepository);
    }
}
