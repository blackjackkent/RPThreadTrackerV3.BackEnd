// <copyright file="IAuthService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using Data;
    using Infrastructure.Data.Entities;
    using Microsoft.AspNetCore.Identity;
    using Models.Configuration;
    using Models.DomainModels;
    using Models.ViewModels.Auth;

    /// <summary>
    /// Service for data manipulation relating to user authentication.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Gets a user by username or email.
        /// </summary>
        /// <param name="usernameOrEmail">A username or email string.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="IdentityUser"/> associated with the given username or email.
        /// </returns>
        Task<IdentityUser> GetUserByUsernameOrEmail(string usernameOrEmail, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Generates a JWT for the given user's claims.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an <see cref="AuthToken"/> containing the JWT information.
        /// </returns>
        Task<AuthToken> GenerateJwt(IdentityUser user, UserManager<IdentityUser> userManager, AppSettings config);

        /// <summary>
        /// Generates a refresh token for the given user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="config">The user manager.</param>
        /// <param name="refreshTokenRepository">The refresh token repository.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an <see cref="AuthToken" /> containing the refresh token information information.
        /// </returns>
        AuthToken GenerateRefreshToken(IdentityUser user, AppSettings config, IRepository<RefreshToken> refreshTokenRepository);

        /// <summary>
        /// Gets the user with whom the given refresh token is associated.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="refreshTokenRepository">The refresh token repository.</param>
        /// <returns>The <see cref="IdentityUser"/> associated with the given refresh token.</returns>
        IdentityUser GetUserForRefreshToken(string refreshToken, IRepository<RefreshToken> refreshTokenRepository);

        /// <summary>
        /// Gets the currently logged in user based on a claims principal.
        /// </summary>
        /// <param name="claimsUser">The claims principal.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="User"/> containing information about the
        /// currently logged in user.
        /// </returns>
        Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper);

        /// <summary>
        /// Gets user profile settings for the given user.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="profileSettingsRepository">The profile settings repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>The profile settings object for the given user.</returns>
        ProfileSettings GetProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper);

        /// <summary>
        /// Updates the passed profile settings.
        /// </summary>
        /// <param name="settings">The model containing profile settings information information.</param>
        /// <param name="profileSettingsRepository">The profile settings repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        void UpdateProfileSettings(ProfileSettings settings, IRepository<ProfileSettingsCollection> profileSettingsRepository, IMapper mapper);

        /// <summary>
        /// Initializes a profile settings record for the given user if it does not exist.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="profileSettingsRepository">The profile settings repository.</param>
        void InitProfileSettings(string userId, IRepository<ProfileSettingsCollection> profileSettingsRepository);

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        /// <param name="email">The email of the user whose password should be reset.</param>
        /// <param name="passwordResetToken">The password reset token associated with the user's password reset request.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="confirmNewPassword">The new password confirmation.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ResetPassword(string email, string passwordResetToken, string newPassword, string confirmNewPassword, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="user">The user to be updated.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="confirmNewPassword">The new password confirmation.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ChangePassword(ClaimsPrincipal user, string currentPassword, string newPassword, string confirmNewPassword, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Changes a user's account information.
        /// </summary>
        /// <param name="user">The user to be updated.</param>
        /// <param name="email">The email the user should be updated to use.</param>
        /// <param name="username">The username the user should be updated to use.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ChangeAccountInformation(ClaimsPrincipal user, string email, string username, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Validates a password for a given user.
        /// </summary>
        /// <param name="user">The user to be validated against.</param>
        /// <param name="password">The password to be validated.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ValidatePassword(IdentityUser user, string password, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IdentityUser> CreateUser(IdentityUser user, string password, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Adds the user to a given role.
        /// </summary>
        /// <param name="user">The user to be updated.</param>
        /// <param name="role">The role the user should be added to.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddUserToRole(IdentityUser user, string role, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Deletes a user's account.
        /// </summary>
        /// <param name="claimsUser">The claims principal.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteAccount(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager);

        /// <summary>
        /// Revokes the given refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to be revoked.</param>
        /// <param name="refreshTokenRepository">The refresh token repository.</param>
        void RevokeRefreshToken(string refreshToken, IRepository<RefreshToken> refreshTokenRepository);

        /// <summary>
        /// Throws an exception if a user already exists with the given username or email.
        /// </summary>
        /// <param name="username">The username to be checked.</param>
        /// <param name="email">The email to be checked.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AssertUserInformationDoesNotExist(string username, string email, UserManager<IdentityUser> userManager);
    }
}
