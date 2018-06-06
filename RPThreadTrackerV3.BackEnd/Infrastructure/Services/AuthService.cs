// <copyright file="AuthService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Data.Entities;
    using Exceptions.Account;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Models.DomainModels;
    using Models.ViewModels.Auth;

    /// <inheritdoc />
    public class AuthService : IAuthService
    {
        /// <inheritdoc />
        /// <exception cref="UserNotFoundException">Thrown if
        /// no user exists with the given username or email.</exception>
        public async Task<IdentityUser> GetUserByUsernameOrEmail(string usernameOrEmail, UserManager<IdentityUser> userManager)
	    {
			var user = await userManager.FindByNameAsync(usernameOrEmail) ?? await userManager.FindByEmailAsync(usernameOrEmail);
	        if (user == null)
	        {
                throw new UserNotFoundException();
	        }
	        return user;
	    }

        /// <inheritdoc />
        public async Task<AuthToken> GenerateJwt(IdentityUser user, UserManager<IdentityUser> userManager, IConfiguration config)
	    {
		    var claims = await GetUserClaims(user, userManager);
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
		    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
	        var expiry = DateTime.UtcNow.AddMinutes(double.Parse(config["Tokens:AccessExpireMinutes"], CultureInfo.CurrentCulture));
            var token = new JwtSecurityToken(
		        config["Tokens:Issuer"],
		        config["Tokens:Audience"],
			    claims,
			    expires: expiry,
			    signingCredentials: creds);
	        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthToken(jwtString, expiry);
	    }

        /// <inheritdoc />
        public AuthToken GenerateRefreshToken(IdentityUser user, IConfiguration config, IRepository<RefreshToken> refreshTokenRepository)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiry = now.AddMinutes(double.Parse(config["Tokens:RefreshExpireMinutes"], CultureInfo.CurrentCulture));
            var jwt = new JwtSecurityToken(
                config["Tokens:Issuer"],
                config["Tokens:Audience"],
                claims,
                expires: expiry,
                signingCredentials: signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            refreshTokenRepository.Create(new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = token,
                IssuedUtc = now,
                ExpiresUtc = expiry,
                UserId = user.Id
            });
            return new AuthToken(token, expiry);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidRefreshTokenException">Thrown if the given refresh token does not
        /// exist or is expired.</exception>
        public IdentityUser GetUserForRefreshToken(string refreshToken, IConfiguration config, IRepository<RefreshToken> refreshTokenRepository)
        {
            var storedToken = refreshTokenRepository.GetWhere(t => t.Token == refreshToken, new List<string> { "User" }).OrderByDescending(t => t.ExpiresUtc).FirstOrDefault();
            var now = DateTime.UtcNow;
            if (storedToken == null || now > storedToken.ExpiresUtc)
            {
                throw new InvalidRefreshTokenException();
            }
            return storedToken.User;
        }

        /// <exception cref="UserNotFoundException">Thrown if no matching user can be found.</exception>
        /// <inheritdoc />
        public async Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper)
	    {
			var identityUser = await userManager.GetUserAsync(claimsUser);
	        if (identityUser == null)
	        {
                throw new UserNotFoundException();
	        }
		    return mapper.Map<User>(identityUser);
	    }

        /// <inheritdoc />
        /// <exception cref="ProfileSettingsNotFoundException">Thrown if no matching profile settings could be found.</exception>
        public ProfileSettings GetProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper)
	    {
		    var settingsEntity = profileSettingsRepository.GetWhere(p => p.UserId == userId).FirstOrDefault();
		    if (settingsEntity == null)
		    {
			    throw new ProfileSettingsNotFoundException();
		    }
		    return mapper.Map<ProfileSettings>(settingsEntity);
	    }

        /// <inheritdoc />
        public void UpdateProfileSettings(ProfileSettings settings, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper)
        {
		    var entity = mapper.Map<ProfileSettingsCollection>(settings);
		    profileSettingsRepository.Update(settings.SettingsId.ToString(CultureInfo.CurrentCulture), entity);
		}

        /// <inheritdoc />
        public void InitProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository)
	    {
	        var existingSettings = profileSettingsRepository.GetWhere(s => s.UserId == userId).FirstOrDefault();
	        if (existingSettings != null)
	        {
	            return;
	        }
		    var settings = new ProfileSettingsCollection
		    {
			    UserId = userId,
			    ShowDashboardThreadDistribution = true
		    };
		    profileSettingsRepository.Create(settings);
	    }

        /// <inheritdoc />
        /// <exception cref="UserNotFoundException">Thrown if a user could not be found for the given email</exception>
        /// <exception cref="InvalidPasswordResetException">Thrown if the password reset request could not be completed.</exception>
        public async Task ResetPassword(string email, string passwordResetToken, string newPassword, UserManager<IdentityUser> userManager)
	    {
		    if (string.IsNullOrEmpty(email))
		    {
			    throw new UserNotFoundException();
		    }
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

        /// <inheritdoc />
        /// <exception cref="InvalidChangePasswordException">Thrown if the change password request was invalid.</exception>
        public async Task ChangePassword(ClaimsPrincipal user, string currentPassword, string newPassword, string confirmNewPassword, UserManager<IdentityUser> userManager)
	    {
		    if (newPassword != confirmNewPassword)
		    {
			    throw new InvalidChangePasswordException(new List<string> { "Passwords do not match." });
		    }
		    var identityUser = await userManager.GetUserAsync(user);
		    var result = await userManager.ChangePasswordAsync(identityUser, currentPassword, newPassword);
			if (!result.Succeeded)
			{
				throw new InvalidChangePasswordException(result.Errors.Select(e => e.Description).ToList());
			}
        }

        /// <inheritdoc />
        /// <exception cref="InvalidAccountInfoUpdateException">Thrown if the account information update could not be completed.</exception>
        public async Task ChangeAccountInformation(ClaimsPrincipal user, string email, string username, UserManager<IdentityUser> userManager)
		{
			var identityUser = await userManager.GetUserAsync(user);
			var errors = new List<string>();
			if (email != identityUser.Email)
			{
				// TODO: update user's email if it's unique
			}
			if (username != identityUser.UserName)
			{
				var existingUser = await userManager.FindByNameAsync(username);
				if (existingUser != null)
				{
					errors.Add("That username is unavailable.");
				}
				else
				{
					identityUser.UserName = username;
					var result = await userManager.UpdateAsync(identityUser);
					if (!result.Succeeded)
					{
						errors.AddRange(result.Errors.Select(e => e.Description));
					}
				}
			}
			if (errors.Any())
			{
				throw new InvalidAccountInfoUpdateException(errors);
			}
		}

        /// <inheritdoc />
        /// <exception cref="InvalidRegistrationException">Thrown if the registration request could not be completed.</exception>
        public async Task CreateUser(IdentityUser user, string password, UserManager<IdentityUser> userManager)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidRegistrationException(result.Errors.Select(e => e.Description).ToList());
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidAccountInfoUpdateException">Thrown if the role update could not be completed.</exception>
        public async Task AddUserToRole(IdentityUser user, string role, UserManager<IdentityUser> userManager)
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidAccountInfoUpdateException(roleResult.Errors.Select(e => e.Description).ToList());
            }
        }

        /// <inheritdoc />
        public void RevokeRefreshToken(string refreshToken, IConfiguration config, IRepository<RefreshToken> refreshTokenRepository)
        {
            var token = refreshTokenRepository.GetWhere(t => t.Token == refreshToken).FirstOrDefault();
            if (token != null)
            {
                refreshTokenRepository.Delete(token);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidRegistrationException">Thrown if the registration request could not be completed.</exception>
        public async Task AssertUserInformationDoesNotExist(string username, string email, UserManager<IdentityUser> userManager)
        {
            var userByUsername = await userManager.FindByNameAsync(username);
            var userByEmail = await userManager.FindByEmailAsync(email);
            if (userByUsername != null || userByEmail != null)
            {
                throw new InvalidRegistrationException(new List<string> { "Error creating account. An account with some or all of this information may already exist." });
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidCredentialException">Thrown if the provided credentials are invalid.</exception>
        public async Task ValidatePassword(IdentityUser user, string password, UserManager<IdentityUser> userManager)
        {
            var verificationResult = await userManager.CheckPasswordAsync(user, password);
            if (!verificationResult)
            {
                throw new InvalidCredentialException();
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
