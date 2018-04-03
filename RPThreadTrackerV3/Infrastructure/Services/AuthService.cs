namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System;
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Linq;
	using System.Security.Claims;
	using System.Text;
	using System.Threading.Tasks;
	using AutoMapper;
	using Data.Entities;
	using Enums;
	using Exceptions;
	using Interfaces.Data;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.IdentityModel.Tokens;
	using Models.DomainModels;
	using Models.ViewModels;

	public class AuthService : IAuthService
    {
	    public async Task<IdentityUser> GetUserByUsernameOrEmail(string usernameOrEmail, UserManager<IdentityUser> userManager)
	    {
			var user = await userManager.FindByNameAsync(usernameOrEmail) ?? await userManager.FindByEmailAsync(usernameOrEmail);
		    return user;
	    }

	    public async Task<JwtSecurityToken> GenerateJwt(IdentityUser user, string key, string issuer, string audience, UserManager<IdentityUser> userManager)
	    {
		    var claims = await GetUserClaims(user, userManager);
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		    var token = new JwtSecurityToken(
			    issuer,
			    audience,
			    claims,
			    expires: DateTime.UtcNow.AddMinutes(15),
			    signingCredentials: creds);
		    return token;
	    }

	    public async Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper)
	    {
			var identityUser = await userManager.GetUserAsync(claimsUser);
		    return identityUser == null ? null : mapper.Map<User>(identityUser);
	    }

	    public ProfileSettings GetProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper)
	    {
		    var settingsEntity = profileSettingsRepository.GetWhere(p => p.UserId == userId).FirstOrDefault();
		    if (settingsEntity == null)
		    {
			    throw new ProfileSettingsNotFoundException();
		    }
		    return mapper.Map<ProfileSettings>(settingsEntity);
	    }

	    public void UpdateProfileSettings(ProfileSettings settings, string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper)
	    {
		    var entity = mapper.Map<ProfileSettingsCollection>(settings);
		    profileSettingsRepository.Update(settings.SettingsId.ToString(), entity);
		}

	    public void InitProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository)
	    {
		    var settings = new ProfileSettingsCollection
		    {
			    UserId = userId,
			    AllowMarkQueued = false,
			    ShowDashboardThreadDistribution = true,
			    UseInvertedTheme = false
		    };
		    profileSettingsRepository.Create(settings);
	    }

	    public async Task ResetPassword(string email, string passwordResetToken, string newPassword, UserManager<IdentityUser> userManager)
	    {
		    var user = await userManager.FindByEmailAsync(email);
		    if (user == null)
		    {
			    throw new UserNotFoundException();
		    }
		    var result = await userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);
		    if (!result.Succeeded)
		    {
			    throw new InvalidPasswordResetException(result.Errors.Select(e => e.Description).ToList());
		    }
		}

	    private static async Task<IEnumerable<Claim>> GetUserClaims(IdentityUser user, UserManager<IdentityUser> userManager)
	    {
			var userClaims = await userManager.GetClaimsAsync(user);
		    var claims = new[]
		    {
			    new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
			    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			    new Claim(JwtRegisteredClaimNames.Email, user.Email),
		    }.Union(userClaims);
		    return claims;
	    }
    }
}
