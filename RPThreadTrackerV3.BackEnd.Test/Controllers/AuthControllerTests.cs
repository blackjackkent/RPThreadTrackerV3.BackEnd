// <copyright file="AuthControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using BackEnd.Controllers;
    using BackEnd.Infrastructure.Data.Entities;
    using BackEnd.Infrastructure.Enums;
    using BackEnd.Infrastructure.Exceptions.Account;
    using BackEnd.Models.Configuration;
    using BackEnd.Models.RequestModels;
    using BackEnd.Models.ViewModels;
    using BackEnd.Models.ViewModels.Auth;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using TestHelpers;
    using Xunit;
    using Entities = BackEnd.Infrastructure.Data.Entities;

    [Trait("Class", "AuthController")]
    public class AuthControllerTests : ControllerTests<AuthController>
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly AppSettings _mockConfig;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IRepository<ProfileSettingsCollection>> _mockProfileSettingsRepository;
        private readonly Mock<IRepository<Entities.RefreshToken>> _mockRefreshTokenRepository;
        private readonly Mock<IEmailClient> _mockEmailClient;

        public AuthControllerTests()
        {
            var mockLogger = new Mock<ILogger<AuthController>>();
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockAuthService = new Mock<IAuthService>();
            _mockProfileSettingsRepository = new Mock<IRepository<ProfileSettingsCollection>>();
            _mockEmailClient = new Mock<IEmailClient>();
            var mockEmailBuilder = new Mock<IEmailBuilder>();
            _mockRefreshTokenRepository = new Mock<IRepository<Entities.RefreshToken>>();
	        _mockConfig = new AppSettings();
			var configWrapper = new Mock<IOptions<AppSettings>>();
	        configWrapper.SetupGet(c => c.Value).Returns(_mockConfig);
            Controller = new AuthController(mockLogger.Object, _mockUserManager.Object, configWrapper.Object, _mockAuthService.Object, _mockProfileSettingsRepository.Object, _mockEmailClient.Object, mockEmailBuilder.Object, _mockRefreshTokenRepository.Object);
        }

        public class CreateToken : AuthControllerTests
        {
            [Fact]
            public async Task ReturnsBadRequestWhenUserNotFound()
            {
                // Arrange
                var model = new LoginRequest
                {
                    Username = "my-username",
                    Password = "my-password"
                };
                _mockAuthService.Setup(s => s.GetUserByUsernameOrEmail("my-username", _mockUserManager.Object)).Throws<UserNotFoundException>();

                // Act
                var result = await Controller.CreateToken(model);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockAuthService.Verify(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig), Times.Never);
                _mockAuthService.Verify(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig, _mockRefreshTokenRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenCredentialsAreInvalid()
            {
                // Arrange
                var model = new LoginRequest
                {
                    Username = "my-username",
                    Password = "my-password"
                };
                var user = new IdentityUser { UserName = "my-username", Id = "12345" };
                _mockAuthService.Setup(s => s.GetUserByUsernameOrEmail("my-username", _mockUserManager.Object)).ReturnsAsync(user);
                _mockAuthService.Setup(s => s.ValidatePassword(user, "my-password", _mockUserManager.Object)).Throws<InvalidCredentialException>();

                // Act
                var result = await Controller.CreateToken(model);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockAuthService.Verify(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig), Times.Never);
                _mockAuthService.Verify(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig, _mockRefreshTokenRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedExceptionThrown()
            {
                // Arrange
                var model = new LoginRequest
                {
                    Username = "my-username",
                    Password = "my-password"
                };
                _mockAuthService
                    .Setup(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.CreateToken(model);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWhenNoExceptionsThrown()
            {
                // Arrange
                var model = new LoginRequest
                {
                    Username = "my-username",
                    Password = "my-password"
                };
                var token = new AuthToken
                {
                    Token = "token",
                    Expiry = 12345
                };
                var refreshToken = new AuthToken
                {
                    Token = "refreshtoken",
                    Expiry = 54321
                };
                _mockAuthService
                    .Setup(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig))
                    .ReturnsAsync(token);
                _mockAuthService
                    .Setup(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig, _mockRefreshTokenRepository.Object))
                    .Returns(refreshToken);

                // Act
                var result = await Controller.CreateToken(model);
                var tokenCollection = ((OkObjectResult)result).Value as AuthTokenCollection;

                // Assert
                tokenCollection.Should().NotBeNull();
                tokenCollection?.Token.Should().Be(token);
                tokenCollection?.RefreshToken.Should().Be(refreshToken);
            }
        }

        public class RefreshToken : AuthControllerTests
        {
            [Fact]
            public async Task ReturnsInvalidTokenWhenTokenIsInvalid()
            {
                // Arrange
                var model = new RefreshTokenRequest
                {
                    RefreshToken = "refreshtoken"
                };
                _mockAuthService
                    .Setup(s => s.GetUserForRefreshToken(model.RefreshToken, _mockRefreshTokenRepository.Object))
                    .Throws<InvalidRefreshTokenException>();

                // Act
                var result = await Controller.RefreshToken(model);

                // Assert
                result.Should().BeOfType<StatusCodeResult>();
                ((StatusCodeResult)result).StatusCode.Should().Be(498);
                _mockAuthService.Verify(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig), Times.Never);
                _mockAuthService.Verify(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig, _mockRefreshTokenRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedExceptionThrown()
            {
                // Arrange
                var model = new RefreshTokenRequest()
                {
                    RefreshToken = "refreshtoken"
                };
                _mockAuthService
                    .Setup(s => s.GetUserForRefreshToken(model.RefreshToken, _mockRefreshTokenRepository.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.RefreshToken(model);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWhenNoExceptionsThrown()
            {
                // Arrange
                var model = new RefreshTokenRequest()
                {
                    RefreshToken = "refreshtoken"
                };
                var token = new AuthToken
                {
                    Token = "token",
                    Expiry = 12345
                };
                var refreshToken = new AuthToken
                {
                    Token = "refreshtoken",
                    Expiry = 54321
                };
                _mockAuthService
                    .Setup(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig))
                    .ReturnsAsync(token);
                _mockAuthService
                    .Setup(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig, _mockRefreshTokenRepository.Object))
                    .Returns(refreshToken);

                // Act
                var result = await Controller.RefreshToken(model);
                var tokenCollection = ((OkObjectResult)result).Value as AuthTokenCollection;

                // Assert
                tokenCollection.Should().NotBeNull();
                tokenCollection?.Token.Should().Be(token);
                tokenCollection?.RefreshToken.Should().Be(refreshToken);
            }
        }

        public class RevokeToken : AuthControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var model = new RefreshTokenRequest
                {
                    RefreshToken = "refreshtoken"
                };
                _mockAuthService.Setup(s => s.RevokeRefreshToken("refreshtoken", _mockRefreshTokenRepository.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.RevokeToken(model);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenNoExceptionsThrown()
            {
                // Arrange
                var model = new RefreshTokenRequest()
                {
                    RefreshToken = "refreshtoken"
                };

                // Act
                var result = Controller.RevokeToken(model);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }

        public class Register : AuthControllerTests
        {
            private readonly RegisterRequest _validRequest;

            public Register()
            {
                _validRequest = new RegisterRequest
                {
                    Email = "me@me.com",
                    Username = "my-username",
                    Password = "my-password",
                    ConfirmPassword = "my-password"
                };
            }

            [Fact]
            public async Task ReturnsBadRequestWhenRegistrationIsInvalid()
            {
                // Arrange
                var model = new Mock<RegisterRequest>();
                var exception = new InvalidRegistrationException(new List<string> { "error1", "error2" });
                model.Setup(m => m.AssertIsValid()).Throws(exception);

                // Act
                var result = await Controller.Register(model.Object);
                var body = ((BadRequestObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                body.Should().HaveCount(2);
                _mockAuthService.Verify(s => s.CreateUser(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object), Times.Never);
                _mockAuthService.Verify(s => s.AddUserToRole(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object), Times.Never);
                _mockAuthService.Verify(s => s.InitProfileSettings(It.IsAny<string>(), _mockProfileSettingsRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenUserInformationExists()
            {
                // Arrange
                var exception = new InvalidRegistrationException(new List<string> { "error1", "error2" });
                _mockAuthService.Setup(s =>
                        s.AssertUserInformationDoesNotExist("my-username", "me@me.com", _mockUserManager.Object))
                    .Throws(exception);

                // Act
                var result = await Controller.Register(_validRequest);
                var body = ((BadRequestObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                body.Should().HaveCount(2);
                _mockAuthService.Verify(s => s.CreateUser(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object), Times.Never);
                _mockAuthService.Verify(s => s.AddUserToRole(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object), Times.Never);
                _mockAuthService.Verify(s => s.InitProfileSettings(It.IsAny<string>(), _mockProfileSettingsRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenUserCreationFails()
            {
                // Arrange
                var exception = new InvalidRegistrationException(new List<string> { "error1", "error2" });
                _mockAuthService.Setup(s =>
                        s.CreateUser(It.Is<IdentityUser>(u => u.UserName == "my-username" && u.Email == "me@me.com"), "my-password", _mockUserManager.Object))
                    .Throws(exception);

                // Act
                var result = await Controller.Register(_validRequest);
                var body = ((BadRequestObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                body.Should().HaveCount(2);
                _mockAuthService.Verify(s => s.AddUserToRole(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object), Times.Never);
                _mockAuthService.Verify(s => s.InitProfileSettings(It.IsAny<string>(), _mockProfileSettingsRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenUserRoleCreationFails()
            {
                // Arrange
                var exception = new InvalidAccountInfoUpdateException(new List<string> { "error1", "error2" });
                _mockAuthService.Setup(s =>
                        s.AddUserToRole(It.IsAny<IdentityUser>(), RoleConstants.USER_ROLE, _mockUserManager.Object))
                    .Throws(exception);

                // Act
                var result = await Controller.Register(_validRequest);
                var body = ((BadRequestObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                body.Should().HaveCount(2);
                _mockAuthService.Verify(s => s.InitProfileSettings(It.IsAny<string>(), _mockProfileSettingsRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedExceptionThrown()
            {
                // Arrange
                _mockAuthService.Setup(s => s.CreateUser(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Register(_validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task CreatesProfileSettingsAndReturnsOkWhenNoExceptionsThrown()
            {
                // Arrange
                var createdUser = new IdentityUser { Id = "12345" };
                _mockAuthService.Setup(s =>
                        s.CreateUser(It.IsAny<IdentityUser>(), It.IsAny<string>(), _mockUserManager.Object))
                    .ReturnsAsync(createdUser);

                // Act
                var result = await Controller.Register(_validRequest);

                // Assert
                result.Should().BeOfType<OkResult>();
                _mockAuthService.Verify(r => r.InitProfileSettings("12345", _mockProfileSettingsRepository.Object), Times.Once);
            }
        }

        public class ForgotPassword : AuthControllerTests
        {
            [Fact]
            public async Task ReturnsOkWhenUserNotFound()
            {
                // Arrange
                var request = new ForgotPasswordRequestModel { Email = "me@me.com" };
                _mockAuthService.Setup(s => s.GetUserByUsernameOrEmail("me@me.com", _mockUserManager.Object))
                    .Throws<UserNotFoundException>();

                // Act
                var result = await Controller.ForgotPassword(request);

                // Assert
                result.Should().BeOfType<OkResult>();
                _mockEmailClient.Verify(c => c.SendEmail(It.IsAny<EmailDto>(), _mockConfig), Times.Never);
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new ForgotPasswordRequestModel { Email = "me@me.com" };
                var user = new IdentityUser { Id = "12345" };
                _mockAuthService.Setup(s => s.GetUserByUsernameOrEmail("me@me.com", _mockUserManager.Object))
                    .Returns(Task.FromResult(user));
                _mockUserManager.Setup(m => m.GeneratePasswordResetTokenAsync(user))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.ForgotPassword(request);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
                _mockEmailClient.Verify(c => c.SendEmail(It.IsAny<EmailDto>(), _mockConfig), Times.Never);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestSuccessful()
            {
                // Arrange
                var request = new ForgotPasswordRequestModel { Email = "me@me.com" };
                var user = new IdentityUser { Id = "12345" };
                _mockConfig.Cors = new CorsAppSettings { CorsUrl = "cors" };
                _mockAuthService.Setup(s => s.GetUserByUsernameOrEmail("me@me.com", _mockUserManager.Object))
                    .Returns(Task.FromResult(user));

                // Act
                var result = await Controller.ForgotPassword(request);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }

        public class ResetPassword : AuthControllerTests
        {
            [Fact]
            public async Task ReturnsBadRequestWhenPasswordResetInvalid()
            {
                // Arrange
                var request = new ResetPasswordRequestModel
                {
                    Code = "code",
                    Email = "me@me.com",
                    NewPassword = "my-password",
                    ConfirmNewPassword = "my-password"
                };
                var exception = new InvalidPasswordResetTokenException(new List<string> { "error1", "error2" });
                _mockAuthService.Setup(s => s.ResetPassword(request.Email, request.Code, request.NewPassword, request.ConfirmNewPassword, _mockUserManager.Object)).ThrowsAsync(exception);

                // Act
                var result = await Controller.ResetPassword(request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task ReturnsBadRequestWhenUserNotFound()
            {
                // Arrange
                var request = new ResetPasswordRequestModel
                {
                    Code = "code",
                    Email = "me@me.com",
                    NewPassword = "my-password",
                    ConfirmNewPassword = "my-password"
                };
                _mockAuthService.Setup(s => s.ResetPassword(request.Email, request.Code, request.NewPassword, request.ConfirmNewPassword, _mockUserManager.Object)).Throws<UserNotFoundException>();

                // Act
                var result = await Controller.ResetPassword(request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new ResetPasswordRequestModel
                {
                    Code = "code",
                    Email = "me@me.com",
                    NewPassword = "my-password",
                    ConfirmNewPassword = "my-password"
                };
                _mockAuthService.Setup(s => s.ResetPassword(request.Email, request.Code, request.NewPassword, request.ConfirmNewPassword, _mockUserManager.Object)).Throws<NullReferenceException>();

                // Act
                var result = await Controller.ResetPassword(request);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestSuccessful()
            {
                // Arrange
                var request = new ResetPasswordRequestModel
                {
                    Code = "code",
                    Email = "me@me.com",
                    NewPassword = "my-password",
                    ConfirmNewPassword = "my-password"
                };

                // Act
                var result = await Controller.ResetPassword(request);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
