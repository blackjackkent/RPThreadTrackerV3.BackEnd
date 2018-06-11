// <copyright file="AuthServiceTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using BackEnd.Infrastructure.Data.Entities;
    using BackEnd.Infrastructure.Exceptions.Account;
    using BackEnd.Infrastructure.Services;
    using BackEnd.Models.Configuration;
    using BackEnd.Models.DomainModels;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;

    [Trait("Class", "AuthService")]
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly AuthService _authService;
        private readonly AppSettings _mockConfig;
        private readonly Mock<IRepository<RefreshToken>> _mockRefreshTokenRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<ProfileSettingsCollection>> _mockProfileSettingsRepository;

        public AuthServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockConfig = new AppSettings();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockRefreshTokenRepository = new Mock<IRepository<RefreshToken>>();
            _mockProfileSettingsRepository = new Mock<IRepository<ProfileSettingsCollection>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<IdentityUser>()))
                .Returns((IdentityUser entity) => new User
                {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    Email = entity.Email
                });
            _mockMapper.Setup(m => m.Map<ProfileSettings>(It.IsAny<ProfileSettingsCollection>()))
                .Returns((ProfileSettingsCollection entity) => new ProfileSettings
                {
                    UserId = entity.UserId,
                    SettingsId = entity.ProfileSettingsCollectionId,
                    ShowDashboardThreadDistribution = entity.ShowDashboardThreadDistribution
                });
            _authService = new AuthService();
        }

        public class GetUserByUsernameOrEmail : AuthServiceTests
        {
            [Fact]
            public async Task GetsUserByUsername()
            {
                // Arrange
                var username = "my-username";
                var user = new IdentityUser
                {
                    Id = "12345"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync(username)).Returns(Task.FromResult(user));

                // Act
                var result = await _authService.GetUserByUsernameOrEmail(username, _mockUserManager.Object);

                // Assert
                result.Id.Should().Be("12345");
            }

            [Fact]
            public async Task GetsUserByEmail()
            {
                // Arrange
                var email = "me@me.com";
                var user = new IdentityUser
                {
                    Id = "12345"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync(email)).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(u => u.FindByEmailAsync(email)).Returns(Task.FromResult(user));

                // Act
                var result = await _authService.GetUserByUsernameOrEmail(email, _mockUserManager.Object);

                // Assert
                result.Id.Should().Be("12345");
            }

            [Fact]
            public async Task ThrowsExceptionIfUserDoesNotExist()
            {
                // Arrange
                var email = "me@me.com";
                _mockUserManager.Setup(u => u.FindByNameAsync(email)).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(u => u.FindByEmailAsync(email)).Returns(Task.FromResult((IdentityUser)null));

                // Act/Assert
                await Assert.ThrowsAsync<UserNotFoundException>(() => _authService.GetUserByUsernameOrEmail(email, _mockUserManager.Object));
            }
        }

        public class GenerateJwt : AuthServiceTests
        {
            [Fact]
            public async Task GeneratesToken()
            {
                // Arrange
                _mockUserManager.Setup(u => u.GetClaimsAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<Claim>());
                _mockConfig.Tokens.Key = "oXHqjGKtAIRBzonnhpmMJuQLoPUd8xH9E1NNlcO5oMhtN";
                _mockConfig.Tokens.AccessExpireMinutes = 12345;
                _mockConfig.Tokens.Audience = "tokenaudience";
                _mockConfig.Tokens.Issuer = "tokenissuer";
                var user = new IdentityUser
                {
                    Id = "12345",
                    UserName = "my-user",
                    Email = "me@me.com"
                };

                // Act
                var result = await _authService.GenerateJwt(user, _mockUserManager.Object, _mockConfig);

                // Assert
                result.Token.Should().NotBe(null);
                result.Expiry.Should().BeGreaterThan(DateTime.UtcNow.Ticks);
            }
        }

        public class GenerateRefreshToken : AuthServiceTests
        {
            [Fact]
            public void GeneratesToken()
            {
                // Arrange
                _mockUserManager.Setup(u => u.GetClaimsAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<Claim>());
                _mockConfig.Tokens.Key = "oXHqjGKtAIRBzonnhpmMJuQLoPUd8xH9E1NNlcO5oMhtN";
                _mockConfig.Tokens.RefreshExpireMinutes = 12345;
                _mockConfig.Tokens.Audience = "tokenaudience";
                _mockConfig.Tokens.Issuer = "tokenissuer"; 
                var user = new IdentityUser
                {
                    Id = "12345",
                    UserName = "my-user",
                    Email = "me@me.com"
                };

                // Act
                var result = _authService.GenerateRefreshToken(user, _mockConfig, _mockRefreshTokenRepository.Object);

                // Assert
                result.Token.Should().NotBe(null);
                result.Expiry.Should().BeGreaterThan(DateTime.UtcNow.Ticks);
                _mockRefreshTokenRepository.Verify(r => r.Create(It.Is<RefreshToken>(t => t.Token == result.Token && t.ExpiresUtc.Ticks == result.Expiry)));
            }
        }

        public class GetUserForRefreshToken : AuthServiceTests
        {
            [Fact]
            public void ReturnsUserIfTokenIsValid()
            {
                // Arrange
                var expires = DateTime.UtcNow.AddDays(2);
                var storedToken = new RefreshToken
                {
                    Token = "token",
                    UserId = "12345",
                    User = new IdentityUser
                    {
                        Id = "12345"
                    },
                    ExpiresUtc = expires
                };
                _mockRefreshTokenRepository.Setup(r =>
                        r.GetWhere(It.Is<Expression<Func<RefreshToken, bool>>>(y => y.Compile()(storedToken)), It.IsAny<List<string>>()))
                    .Returns(new List<RefreshToken> { storedToken });

                // Act
                var result = _authService.GetUserForRefreshToken("token", _mockRefreshTokenRepository.Object);

                // Assert
                result.Id.Should().Be("12345");
            }

            [Fact]
            public void ThrowsExceptionIfTokenDoesNotExist()
            {
                // Arrange
                _mockRefreshTokenRepository.Setup(r =>
                        r.GetWhere(It.IsAny<Expression<Func<RefreshToken, bool>>>(), It.IsAny<List<string>>()))
                    .Returns(new List<RefreshToken>());

                // Act
                Assert.Throws<InvalidRefreshTokenException>(() => _authService.GetUserForRefreshToken("token", _mockRefreshTokenRepository.Object));
            }

            [Fact]
            public void ThrowsExceptionIfTokenIsExpired()
            {
                // Arrange
                var expires = DateTime.UtcNow.AddDays(-2);
                var storedToken = new RefreshToken
                {
                    Token = "token",
                    UserId = "12345",
                    User = new IdentityUser
                    {
                        Id = "12345"
                    },
                    ExpiresUtc = expires
                };
                _mockRefreshTokenRepository.Setup(r =>
                        r.GetWhere(It.Is<Expression<Func<RefreshToken, bool>>>(y => y.Compile()(storedToken)), It.IsAny<List<string>>()))
                    .Returns(new List<RefreshToken> { storedToken });

                // Act
                Assert.Throws<InvalidRefreshTokenException>(() => _authService.GetUserForRefreshToken("token", _mockRefreshTokenRepository.Object));
            }
        }

        public class GetCurrentUser : AuthServiceTests
        {
            [Fact]
            public async Task ReturnsUserWhenFound()
            {
                // Arrange
                var userEntity = new IdentityUser
                {
                    Id = "12345"
                };
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userEntity);

                // Act
                var result = await _authService.GetCurrentUser(new ClaimsPrincipal(), _mockUserManager.Object, _mockMapper.Object);

                // Assert
                result.Id.Should().Be("12345");
            }

            [Fact]
            public async Task ThrowsExceptionWhenNotFound()
            {
                // Arrange
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

                // Act/Assert
                await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authService.GetCurrentUser(new ClaimsPrincipal(), _mockUserManager.Object, _mockMapper.Object));
            }
        }

        public class GetProfileSettings : AuthServiceTests
        {
            [Fact]
            public void ReturnsSettingsWhenFound()
            {
                // Arrange
                var settingsEntity = new ProfileSettingsCollection
                {
                    ProfileSettingsCollectionId = 12345,
                    UserId = "13579"
                };
                _mockProfileSettingsRepository.Setup(r =>
                        r.GetWhere(It.Is<Expression<Func<ProfileSettingsCollection, bool>>>(y => y.Compile()(settingsEntity)), It.IsAny<List<string>>()))
                    .Returns(new List<ProfileSettingsCollection> { settingsEntity });

                // Act
                var result = _authService.GetProfileSettings("13579", _mockProfileSettingsRepository.Object, _mockMapper.Object);

                // Assert
                result.SettingsId.Should().Be(12345);
            }

            [Fact]
            public void ThrowsExceptionWhenNotFound()
            {
                // Arrange
                _mockProfileSettingsRepository.Setup(r => r.GetWhere(It.IsAny<Expression<Func<ProfileSettingsCollection, bool>>>(), It.IsAny<List<string>>()))
                    .Returns(new List<ProfileSettingsCollection>());

                // Act/Assert
                Assert.Throws<ProfileSettingsNotFoundException>(() => _authService.GetProfileSettings("13579", _mockProfileSettingsRepository.Object, _mockMapper.Object));
            }
        }

        public class UpdateProfileSettings : AuthServiceTests
        {
            [Fact]
            public void UpdatesSettingsInRepository()
            {
                // Arrange
                var settingsEntity = new ProfileSettingsCollection
                {
                    ProfileSettingsCollectionId = 12345
                };
                var settingsModel = new ProfileSettings
                {
                    SettingsId = 12345
                };
                _mockMapper.Setup(m => m.Map<ProfileSettingsCollection>(settingsModel)).Returns(settingsEntity);

                // Act
                _authService.UpdateProfileSettings(settingsModel, _mockProfileSettingsRepository.Object, _mockMapper.Object);

                // Assert
                _mockProfileSettingsRepository.Verify(r => r.Update("12345", settingsEntity), Times.Once);
            }
        }

        public class InitProfileSettings : AuthServiceTests
        {
            [Fact]
            public void DoesNothingIfSettingsAlreadyExist()
            {
                // Arrange
                var settingsEntity = new ProfileSettingsCollection
                {
                    ProfileSettingsCollectionId = 12345,
                    UserId = "13579"
                };
                _mockProfileSettingsRepository.Setup(r =>
                        r.GetWhere(It.Is<Expression<Func<ProfileSettingsCollection, bool>>>(y => y.Compile()(settingsEntity)), It.IsAny<List<string>>()))
                    .Returns(new List<ProfileSettingsCollection> { settingsEntity });

                // Act
                _authService.InitProfileSettings("13579", _mockProfileSettingsRepository.Object);

                // Assert
                _mockProfileSettingsRepository.Verify(r => r.Create(It.IsAny<ProfileSettingsCollection>()), Times.Never);
            }

            [Fact]
            public void CreatesSettingsIfNotAlreadyExisting()
            {
                // Arrange
                _mockProfileSettingsRepository.Setup(r =>
                        r.GetWhere(It.IsAny<Expression<Func<ProfileSettingsCollection, bool>>>(), It.IsAny<List<string>>()))
                    .Returns(new List<ProfileSettingsCollection>());

                // Act
                _authService.InitProfileSettings("12345", _mockProfileSettingsRepository.Object);

                // Assert
                _mockProfileSettingsRepository.Verify(r => r.Create(It.Is<ProfileSettingsCollection>(c => c.UserId == "12345" && c.ShowDashboardThreadDistribution)), Times.Once);
            }
        }

        public class ResetPassword : AuthServiceTests
        {
            [Fact]
            public async Task ThrowsExceptionIfEmailNotProvided()
            {
                // Act/Assert
                await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authService.ResetPassword(null, "12345", "mypassword", _mockUserManager.Object));
            }

            [Fact]
            public async Task ThrowsExceptionIfUserNotFound()
            {
                // Arrange
                _mockUserManager.Setup(m => m.FindByEmailAsync("me@me.com")).Returns(Task.FromResult((IdentityUser)null));

                // Act/Assert
                await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authService.ResetPassword("me@me.com", "12345", "mypassword", _mockUserManager.Object));
            }

            [Fact]
            public async Task ThrowsExceptionIfResetUnsuccessful()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345"
                };
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.FindByEmailAsync("me@me.com")).Returns(Task.FromResult(user));
                _mockUserManager.Setup(m => m.ResetPasswordAsync(user, "12345", "mypassword"))
                    .Returns(Task.FromResult(failureResult));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidPasswordResetException>(async () => await _authService.ResetPassword("me@me.com", "12345", "mypassword", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(2);
                ex.Errors.Should().Contain("Test Error 1");
                ex.Errors.Should().Contain("Test Error 2");
            }

            [Fact]
            public async Task ThrowsNoExceptionIfResetSuccessful()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345"
                };
                _mockUserManager.Setup(m => m.FindByEmailAsync("me@me.com")).Returns(Task.FromResult(user));
                _mockUserManager.Setup(m => m.ResetPasswordAsync(user, "12345", "mypassword"))
                    .Returns(Task.FromResult(IdentityResult.Success));

                // Act
                await _authService.ResetPassword("me@me.com", "12345", "mypassword", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.ResetPasswordAsync(user, "12345", "mypassword"), Times.Once);
            }
        }

        public class ChangePassword : AuthServiceTests
        {
            [Fact]
            public async Task ThrowsExceptionIfPasswordsDoNotMatch()
            {
                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidChangePasswordException>(async () => await _authService.ChangePassword(new ClaimsPrincipal(), "test1", "test2", "test3", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(1);
                ex.Errors.Should().Contain("Passwords do not match.");
            }

            [Fact]
            public async Task ThrowsExceptionIfChangeUnsuccessful()
            {
                // Arrange
                var user = new ClaimsPrincipal();
                var identityUser = new IdentityUser
                {
                    Id = "13579"
                };
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.GetUserAsync(user)).Returns(Task.FromResult(identityUser));
                _mockUserManager.Setup(m => m.ChangePasswordAsync(identityUser, "12345", "23456"))
                    .Returns(Task.FromResult(failureResult));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidChangePasswordException>(async () => await _authService.ChangePassword(user, "12345", "23456", "23456", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(2);
                ex.Errors.Should().Contain("Test Error 1");
                ex.Errors.Should().Contain("Test Error 2");
            }

            [Fact]
            public async Task ThrowsNoExceptionIfChangeSuccessful()
            {
                // Arrange
                var user = new ClaimsPrincipal();
                var identityUser = new IdentityUser
                {
                    Id = "13579"
                };
                _mockUserManager.Setup(m => m.GetUserAsync(user)).Returns(Task.FromResult(identityUser));
                _mockUserManager.Setup(m => m.ChangePasswordAsync(identityUser, "12345", "23456"))
                    .Returns(Task.FromResult(IdentityResult.Success));

                // Act
                await _authService.ChangePassword(user, "12345", "23456", "23456", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.ChangePasswordAsync(identityUser, "12345", "23456"), Times.Once);
            }
        }

        public class ChangeAccountInformation : AuthServiceTests
        {
            [Fact]
            public async Task DoesNotUpdateUsernameIfNotChanged()
            {
                // Arrange
                var currentUser = new IdentityUser
                {
                    Id = "12345",
                    UserName = "current-username"
                };
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(currentUser));

                // Act
                await _authService.ChangeAccountInformation(new ClaimsPrincipal(), "me@me.com", "current-username", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.FindByNameAsync(It.IsAny<string>()), Times.Never());
                _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<IdentityUser>()), Times.Never());
            }

            [Fact]
            public async Task ThrowsExceptionIfChangingToExistingUsername()
            {
                // Arrange
                var currentUser = new IdentityUser
                {
                    Id = "12345",
                    UserName = "current-username"
                };
                var existingOtherUser = new IdentityUser("new-username");
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(currentUser));
                _mockUserManager.Setup(m => m.FindByNameAsync("new-username")).Returns(Task.FromResult(existingOtherUser));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidAccountInfoUpdateException>(async () => await _authService.ChangeAccountInformation(new ClaimsPrincipal(), "test@test.com", "new-username", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(1);
                ex.Errors.Should().Contain("That username is unavailable.");
            }

            [Fact]
            public async Task ThrowsExceptionIfUsernameChangeFailed()
            {
                // Arrange
                var currentUser = new IdentityUser
                {
                    Id = "12345",
                    UserName = "current-username"
                };
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(currentUser));
                _mockUserManager.Setup(m => m.FindByNameAsync("new-username")).Returns(Task.FromResult((IdentityUser)null));
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.UpdateAsync(It.Is<IdentityUser>(u => u.Id == "12345" && u.UserName == "new-username")))
                    .Returns(Task.FromResult(failureResult));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidAccountInfoUpdateException>(async () => await _authService.ChangeAccountInformation(new ClaimsPrincipal(), "test@test.com", "new-username", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(2);
                ex.Errors.Should().Contain("Test Error 1");
                ex.Errors.Should().Contain("Test Error 2");
            }

            [Fact]
            public async Task ThrowsNoExceptionIfUsernameChangeSucceeded()
            {
                // Arrange
                var currentUser = new IdentityUser
                {
                    Id = "12345",
                    UserName = "current-username"
                };
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(currentUser));
                _mockUserManager.Setup(m => m.FindByNameAsync("new-username")).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(m => m.UpdateAsync(It.Is<IdentityUser>(u => u.Id == "12345" && u.UserName == "new-username")))
                    .Returns(Task.FromResult(IdentityResult.Success));

                // Act
                await _authService.ChangeAccountInformation(new ClaimsPrincipal(), "test@test.com", "new-username", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.UpdateAsync(It.Is<IdentityUser>(u => u.Id == "12345" && u.UserName == "new-username")), Times.Once());
            }
        }

        public class CreateUser : AuthServiceTests
        {
            [Fact]
            public async Task CreatesUserSuccessfully()
            {
                // Arrange
                _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

                // Act
                await _authService.CreateUser(new IdentityUser("my-username"), "my-password", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.CreateAsync(It.Is<IdentityUser>(u => u.UserName == "my-username"), "my-password"), Times.Once);
            }

            [Fact]
            public async Task ThrowsExceptionIfCreationUnsuccessful()
            {
                // Arrange
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(failureResult));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidRegistrationException>(async () => await _authService.CreateUser(new IdentityUser("my-username"), "my-password", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(2);
                ex.Errors.Should().Contain("Test Error 1");
                ex.Errors.Should().Contain("Test Error 2");
            }
        }

        public class AddUserToRole : AuthServiceTests
        {
            [Fact]
            public async Task AddsToRoleSuccessfully()
            {
                // Arrange
                _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

                // Act
                await _authService.AddUserToRole(new IdentityUser("my-username"), "my-role", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.AddToRoleAsync(It.Is<IdentityUser>(u => u.UserName == "my-username"), "my-role"), Times.Once);
            }

            [Fact]
            public async Task ThrowsExceptionIfAddingToRoleUnsuccessful()
            {
                // Arrange
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(failureResult));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidAccountInfoUpdateException>(async () => await _authService.AddUserToRole(new IdentityUser("my-username"), "my-password", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(2);
                ex.Errors.Should().Contain("Test Error 1");
                ex.Errors.Should().Contain("Test Error 2");
            }
        }

        public class RevokeRefreshToken : AuthServiceTests
        {
            [Fact]
            public void DoesNothingIfTokenDoesNotExist()
            {
                // Arrange
                _mockRefreshTokenRepository.Setup(r => r.GetWhere(It.IsAny<Expression<Func<RefreshToken, bool>>>(), It.IsAny<List<string>>()))
                    .Returns(new List<RefreshToken>());

                // Act
                _authService.RevokeRefreshToken("token", _mockRefreshTokenRepository.Object);

                // Assert
                _mockRefreshTokenRepository.Verify(r => r.Delete(It.IsAny<RefreshToken>()), Times.Never);
            }

            [Fact]
            public void DeletesTokenIfTokenExists()
            {
                // Arrange
                var token = new RefreshToken
                {
                    Id = "12345",
                    Token = "token",
                    UserId = "13579"
                };
                _mockRefreshTokenRepository.Setup(r =>
                        r.GetWhere(It.Is<Expression<Func<RefreshToken, bool>>>(y => y.Compile()(token)), It.IsAny<List<string>>()))
                    .Returns(new List<RefreshToken> { token });

                // Act
                _authService.RevokeRefreshToken("token", _mockRefreshTokenRepository.Object);

                // Assert
                _mockRefreshTokenRepository.Verify(r => r.Delete(token), Times.Once);
            }
        }

        public class AssertUserInformationDoesNotExist : AuthServiceTests
        {
            [Fact]
            public async Task ThrowsExceptionIfUsernameExists()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345",
                    UserName = "my-username"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync("my-username")).Returns(Task.FromResult(user));
                _mockUserManager.Setup(u => u.FindByEmailAsync("my@email.com")).Returns(Task.FromResult((IdentityUser)null));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidRegistrationException>(async () => await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(1);
                ex.Errors.Should().Contain("Error creating account. An account with some or all of this information may already exist.");
            }

            [Fact]
            public async Task ThrowsExceptionIfEmailExists()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345",
                    Email = "my@email.com"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync("my-username")).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(u => u.FindByEmailAsync("my@email.com")).Returns(Task.FromResult(user));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidRegistrationException>(async () => await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(1);
                ex.Errors.Should().Contain("Error creating account. An account with some or all of this information may already exist.");
            }

            [Fact]
            public async Task ThrowsExceptionIfEmailAndUsernameExist()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345",
                    UserName = "my-username",
                    Email = "my@email.com"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync("my-username")).Returns(Task.FromResult(user));
                _mockUserManager.Setup(u => u.FindByEmailAsync("my@email.com")).Returns(Task.FromResult(user));

                // Act/Assert
                var ex = await Assert.ThrowsAsync<InvalidRegistrationException>(async () => await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object));
                ex.Errors.Should().HaveCount(1);
                ex.Errors.Should().Contain("Error creating account. An account with some or all of this information may already exist.");
            }

            [Fact]
            public async Task ThrowsNoExceptionIfEmailAndUsernameDoNotExist()
            {
                // Arrange
                _mockUserManager.Setup(u => u.FindByNameAsync("my-username")).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(u => u.FindByEmailAsync("my@email.com")).Returns(Task.FromResult((IdentityUser)null));

                // Act/Assert
                await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object);
            }
        }

        public class ValidatePassword : AuthServiceTests
        {
            [Fact]
            public async Task ValidatesSuccessfully()
            {
                // Arrange
                _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));

                // Act
                await _authService.ValidatePassword(new IdentityUser("my-username"), "my-password", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.CheckPasswordAsync(It.Is<IdentityUser>(u => u.UserName == "my-username"), "my-password"), Times.Once);
            }

            [Fact]
            public async Task ThrowsExceptionIfValidationUnsuccessful()
            {
                // Arrange
                _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

                // Act/Assert
                await Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authService.ValidatePassword(new IdentityUser("my-username"), "my-password", _mockUserManager.Object));
            }
        }
    }
}
