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
    using BackEnd.Models.RequestModels;
    using BackEnd.Models.ViewModels.Auth;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using Entities = BackEnd.Infrastructure.Data.Entities;

    [Trait("Class", "AuthController")]
    public class AuthControllerTests : ControllerTests<AuthController>
    {
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<IConfigurationService> _mockConfig;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IRepository<ProfileSettingsCollection>> _mockProfileSettingsRepository;
        private readonly Mock<IEmailClient> _mockEmailClient;
        private readonly Mock<IEmailBuilder> _mockEmailBuilder;
        private readonly Mock<IRepository<Entities.RefreshToken>> _mockRefreshTokenRepository;

        public AuthControllerTests()
        {
            _mockLogger = new Mock<ILogger<AuthController>>();
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockConfig = new Mock<IConfigurationService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockProfileSettingsRepository = new Mock<IRepository<ProfileSettingsCollection>>();
            _mockEmailClient = new Mock<IEmailClient>();
            _mockEmailBuilder = new Mock<IEmailBuilder>();
            _mockRefreshTokenRepository = new Mock<IRepository<Entities.RefreshToken>>();
            Controller = new AuthController(_mockLogger.Object, _mockUserManager.Object, _mockConfig.Object, _mockAuthService.Object, _mockProfileSettingsRepository.Object, _mockEmailClient.Object, _mockEmailBuilder.Object, _mockRefreshTokenRepository.Object);
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
                    .Setup(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.CreateToken(model);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
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
                    .Setup(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig.Object))
                    .ReturnsAsync(token);
                _mockAuthService
                    .Setup(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig.Object, _mockRefreshTokenRepository.Object))
                    .Returns(refreshToken);

                // Act
                var result = await Controller.CreateToken(model);
                var tokenCollection = ((OkObjectResult)result).Value as AuthTokenCollection;

                // Assert
                tokenCollection.Should().NotBeNull();
                tokenCollection.Token.Should().Be(token);
                tokenCollection.RefreshToken.Should().Be(refreshToken);
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
                    .Setup(s => s.GenerateJwt(It.IsAny<IdentityUser>(), _mockUserManager.Object, _mockConfig.Object))
                    .ReturnsAsync(token);
                _mockAuthService
                    .Setup(s => s.GenerateRefreshToken(It.IsAny<IdentityUser>(), _mockConfig.Object, _mockRefreshTokenRepository.Object))
                    .Returns(refreshToken);

                // Act
                var result = await Controller.RefreshToken(model);
                var tokenCollection = ((OkObjectResult)result).Value as AuthTokenCollection;

                // Assert
                tokenCollection.Should().NotBeNull();
                tokenCollection.Token.Should().Be(token);
                tokenCollection.RefreshToken.Should().Be(refreshToken);
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
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
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
            private RegisterRequest _validRequest;

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
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
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
                _mockAuthService.Verify(r => r.InitProfileSettings("12345", _mockProfileSettingsRepository.Object), Times.Once);
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
