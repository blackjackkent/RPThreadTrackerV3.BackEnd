﻿// <copyright file="AuthServiceTests.cs" company="Rosalind Wills">
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
    using Microsoft.AspNetCore.Identity;
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
                    ShowDashboardThreadDistribution = entity.ShowDashboardThreadDistribution,
                    LastNewsReadDate = entity.LastNewsReadDate,
					ThreadTablePageSize = entity.ThreadTablePageSize,
                    #pragma warning disable 618
                    AllowMarkQueued = entity.AllowMarkQueued,
                    UseInvertedTheme = entity.UseInvertedTheme
                    #pragma warning restore 618
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
            public void ThrowsExceptionIfUserDoesNotExist()
            {
                // Arrange
                var email = "me@me.com";
                _mockUserManager.Setup(u => u.FindByNameAsync(email)).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(u => u.FindByEmailAsync(email)).Returns(Task.FromResult((IdentityUser)null));

                // Act
                Func<Task> action = async () => await _authService.GetUserByUsernameOrEmail(email, _mockUserManager.Object);

                // Assert
                action.Should().Throw<UserNotFoundException>();
            }
        }

        public class GenerateJwt : AuthServiceTests
        {
            [Fact]
            public async Task GeneratesToken()
            {
                // Arrange
                _mockUserManager.Setup(u => u.GetClaimsAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<Claim>());
	            _mockConfig.Tokens = new TokensAppSettings
	            {
		            Key = "oXHqjGKtAIRBzonnhpmMJuQLoPUd8xH9E1NNlcO5oMhtN",
		            AccessExpireMinutes = 12345,
		            Audience = "tokenaudience",
		            Issuer = "tokenissuer"
	            };
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
	            _mockConfig.Tokens = new TokensAppSettings
	            {
		            Key = "oXHqjGKtAIRBzonnhpmMJuQLoPUd8xH9E1NNlcO5oMhtN",
		            RefreshExpireMinutes = 12345,
		            Audience = "tokenaudience",
		            Issuer = "tokenissuer"
	            };

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
                Action action = () => _authService.GetUserForRefreshToken("token", _mockRefreshTokenRepository.Object);

                // Assert
                action.Should().Throw<InvalidRefreshTokenException>();
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
                Action action = () => _authService.GetUserForRefreshToken("token", _mockRefreshTokenRepository.Object);

                // Assert
                action.Should().Throw<InvalidRefreshTokenException>();
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
            public void ThrowsExceptionWhenNotFound()
            {
                // Arrange
                _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

                // Act
                Func<Task> action = async () => await _authService.GetCurrentUser(new ClaimsPrincipal(), _mockUserManager.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<UserNotFoundException>();
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

                // Act
                Action action = () => _authService.GetProfileSettings("13579", _mockProfileSettingsRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<ProfileSettingsNotFoundException>();
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
	            var comparisonDateTicks = DateTime.UtcNow.Ticks;

                // Act
                _authService.InitProfileSettings("12345", _mockProfileSettingsRepository.Object);

                // Assert
                _mockProfileSettingsRepository.Verify(r => r.Create(It.Is<ProfileSettingsCollection>(c => c.UserId == "12345" && c.ShowDashboardThreadDistribution && c.LastNewsReadDate.GetValueOrDefault().Ticks > comparisonDateTicks)), Times.Once);
            }
        }

        public class ResetPassword : AuthServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfPasswordsDoNotMatch()
            {
                // Act
                Func<Task> action = async () => await _authService.ResetPassword(null, "12345", "mypassword", "my-password", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidChangePasswordException>();
            }

            [Fact]
            public void ThrowsExceptionIfEmailNotProvided()
            {
                // Act
                Func<Task> action = async () => await _authService.ResetPassword(null, "12345", "mypassword", "mypassword", _mockUserManager.Object);

                // Assert
                action.Should().Throw<UserNotFoundException>();
            }

            [Fact]
            public void ThrowsExceptionIfUserNotFound()
            {
                // Arrange
                _mockUserManager.Setup(m => m.FindByEmailAsync("me@me.com")).Returns(Task.FromResult((IdentityUser)null));

                // Act
                Func<Task> action = async () => await _authService.ResetPassword("me@me.com", "12345", "mypassword", "mypassword", _mockUserManager.Object);

                // Assert
                action.Should().Throw<UserNotFoundException>();
            }

            [Fact]
            public void ThrowsExceptionIfTokenInvalid()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345"
                };
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Code = "InvalidToken", Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.FindByEmailAsync("me@me.com")).Returns(Task.FromResult(user));
                _mockUserManager.Setup(m => m.ResetPasswordAsync(user, "12345", "mypassword"))
                    .Returns(Task.FromResult(failureResult));

                // Act
                Func<Task> action = async () => await _authService.ResetPassword("me@me.com", "12345", "mypassword", "mypassword", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidPasswordResetTokenException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
            }

            [Fact]
            public void ThrowsExceptionIfTokenValidAndPasswordInsecure()
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

                // Act
                Func<Task> action = async () => await _authService.ResetPassword("me@me.com", "12345", "mypassword", "mypassword", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidChangePasswordException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
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
                await _authService.ResetPassword("me@me.com", "12345", "mypassword", "mypassword", _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.ResetPasswordAsync(user, "12345", "mypassword"), Times.Once);
            }
        }

        public class ChangePassword : AuthServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfPasswordsDoNotMatch()
            {
                // Act
                Func<Task> action = async () => await _authService.ChangePassword(new ClaimsPrincipal(), "test1", "test2", "test3", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidChangePasswordException>()
                    .Which.Errors.Should().HaveCount(1)
                    .And.Contain("Passwords do not match.");
            }

            [Fact]
            public void ThrowsExceptionIfChangeUnsuccessful()
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

                // Act
                Func<Task> action = async () => await _authService.ChangePassword(user, "12345", "23456", "23456", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidChangePasswordException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
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
            public void ThrowsExceptionIfChangingToExistingUsername()
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

                // Act
                Func<Task> action = async () => await _authService.ChangeAccountInformation(new ClaimsPrincipal(), "test@test.com", "new-username", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidAccountInfoUpdateException>()
                    .Which.Errors.Should().HaveCount(1)
                    .And.Contain("That username is unavailable.");
            }

            [Fact]
            public void ThrowsExceptionIfUsernameChangeFailed()
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

                // Act
                Func<Task> action = async () => await _authService.ChangeAccountInformation(new ClaimsPrincipal(), "test@test.com", "new-username", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidAccountInfoUpdateException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
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
            public void ThrowsExceptionIfCreationUnsuccessful()
            {
                // Arrange
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(failureResult));

                // Act
                Func<Task> action = async () => await _authService.CreateUser(new IdentityUser("my-username"), "my-password", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidRegistrationException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
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
            public void ThrowsExceptionIfAddingToRoleUnsuccessful()
            {
                // Arrange
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(failureResult));

                // Act
                Func<Task> action = async () => await _authService.AddUserToRole(new IdentityUser("my-username"), "my-password", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidAccountInfoUpdateException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
            }
        }

        public class DeleteAccount : AuthServiceTests
        {
            [Fact]
            public async Task DeletesSuccessfully()
            {
                // Arrange
                var user = new ClaimsPrincipal();
                _mockUserManager.Setup(m => m.DeleteAsync(It.IsAny<IdentityUser>())).Returns(Task.FromResult(IdentityResult.Success));
                _mockUserManager.Setup(m => m.GetUserAsync(It.Is<ClaimsPrincipal>(p => ReferenceEquals(p, user))))
                    .Returns(Task.FromResult(new IdentityUser("my-username")));

                // Act
                await _authService.DeleteAccount(user, _mockUserManager.Object);

                // Assert
                _mockUserManager.Verify(m => m.DeleteAsync(It.Is<IdentityUser>(u => u.UserName == "my-username")), Times.Once);
            }

            [Fact]
            public void ThrowsExceptionIfDeletionUnsuccessful()
            {
                // Arrange
                var user = new ClaimsPrincipal();
                _mockUserManager.Setup(m => m.GetUserAsync(It.Is<ClaimsPrincipal>(p => ReferenceEquals(p, user))))
                    .Returns(Task.FromResult(new IdentityUser("my-username")));
                var failureResult = IdentityResult.Failed(new List<IdentityError>
                {
                    new IdentityError { Description = "Test Error 1" },
                    new IdentityError { Description = "Test Error 2" }
                }.ToArray());
                _mockUserManager.Setup(m => m.DeleteAsync(It.IsAny<IdentityUser>()))
                    .Returns(Task.FromResult(failureResult));

                // Act
                Func<Task> action = async () => await _authService.DeleteAccount(user, _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidAccountDeletionException>()
                    .Which.Errors.Should().HaveCount(2)
                    .And.Contain("Test Error 1")
                    .And.Contain("Test Error 2");
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
            public void ThrowsExceptionIfUsernameExists()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345",
                    UserName = "my-username"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync("my-username")).Returns(Task.FromResult(user));
                _mockUserManager.Setup(u => u.FindByEmailAsync("my@email.com")).Returns(Task.FromResult((IdentityUser)null));

                // Act
                Func<Task> action = async () => await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidRegistrationException>()
                    .Which.Errors.Should().HaveCount(1)
                    .And.Contain("Error creating account. An account with some or all of this information may already exist.");
            }

            [Fact]
            public void ThrowsExceptionIfEmailExists()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Id = "12345",
                    Email = "my@email.com"
                };
                _mockUserManager.Setup(u => u.FindByNameAsync("my-username")).Returns(Task.FromResult((IdentityUser)null));
                _mockUserManager.Setup(u => u.FindByEmailAsync("my@email.com")).Returns(Task.FromResult(user));

                // Act
                Func<Task> action = async () => await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidRegistrationException>()
                    .Which.Errors.Should().HaveCount(1)
                    .And.Contain("Error creating account. An account with some or all of this information may already exist.");
            }

            [Fact]
            public void ThrowsExceptionIfEmailAndUsernameExist()
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

                // Act
                Func<Task> action = async () => await _authService.AssertUserInformationDoesNotExist("my-username", "my@email.com", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidRegistrationException>()
                    .Which.Errors.Should().HaveCount(1)
                    .And.Contain("Error creating account. An account with some or all of this information may already exist.");
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
            public void ThrowsExceptionIfValidationUnsuccessful()
            {
                // Arrange
                _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

                // Act
                Func<Task> action = async () => await _authService.ValidatePassword(new IdentityUser("my-username"), "my-password", _mockUserManager.Object);

                // Assert
                action.Should().Throw<InvalidCredentialException>();
            }
        }
    }
}
