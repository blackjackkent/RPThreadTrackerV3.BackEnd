// <copyright file="UserControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using BackEnd.Controllers;
    using BackEnd.Infrastructure.Exceptions.Account;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.RequestModels;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "UserController")]
    public class UserControllerTests : ControllerTests<UserController>
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAuthService> _mockAuthService;

        public UserControllerTests()
        {
            var mockLogger = new Mock<ILogger<UserController>>();
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns((User model) => new UserDto
                {
                    Id = model.Id,
                    Email = model.Email,
                    UserName = model.UserName
                });
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserDto>()))
                .Returns((UserDto dto) => new User
                {
                    Id = dto.Id,
                    Email = dto.Email,
                    UserName = dto.UserName
                });
            _mockAuthService = new Mock<IAuthService>();
            Controller = new UserController(mockLogger.Object, _mockUserManager.Object, _mockMapper.Object, _mockAuthService.Object);
            InitControllerContext();
        }

        public class GetUser : UserControllerTests
        {
            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockAuthService.Setup(s => s.GetCurrentUser(It.IsAny<ClaimsPrincipal>(), _mockUserManager.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Get();

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsNotFoundWhenUserNotFound()
            {
                // Arrange
                _mockAuthService.Setup(s => s.GetCurrentUser(It.IsAny<ClaimsPrincipal>(), _mockUserManager.Object, _mockMapper.Object))
                    .Throws<UserNotFoundException>();

                // Act
                var result = await Controller.Get();

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task ReturnsOkResponseWithUserWhenRequestSuccessful()
            {
                // Arrange
                var user = new User { Email = "me@me.com" };
                _mockAuthService.Setup(s =>
                        s.GetCurrentUser(It.IsAny<ClaimsPrincipal>(), _mockUserManager.Object, _mockMapper.Object))
                    .Returns(Task.FromResult(user));

                // Act
                var result = await Controller.Get();
                var body = ((OkObjectResult)result).Value as UserDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.Email.Should().Be("me@me.com");
            }
        }

        public class ChangePassword : UserControllerTests
        {
            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new ChangePasswordRequestModel
                {
                    ConfirmNewPassword = "my-new-password",
                    NewPassword = "my-new-password",
                    CurrentPassword = "my-old-password"
                };
                _mockAuthService.Setup(s => s.ChangePassword(It.IsAny<ClaimsPrincipal>(), "my-old-password", "my-new-password", "my-new-password", _mockUserManager.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.ChangePassword(request);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenRequestIsInvalid()
            {
                // Arrange
                var request = new ChangePasswordRequestModel
                {
                    ConfirmNewPassword = "my-new-password",
                    NewPassword = "my-new-password",
                    CurrentPassword = "my-old-password"
                };
                var exception = new InvalidChangePasswordException(new List<string> { "error1", "error2" });
                _mockAuthService.Setup(s => s.ChangePassword(It.IsAny<ClaimsPrincipal>(), "my-old-password", "my-new-password", "my-new-password", _mockUserManager.Object))
                    .Throws(exception);

                // Act
                var result = await Controller.ChangePassword(request);
                var body = ((BadRequestObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                body.Should().HaveCount(2);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestSuccessful()
            {
                // Arrange
                var request = new ChangePasswordRequestModel
                {
                    ConfirmNewPassword = "my-new-password",
                    NewPassword = "my-new-password",
                    CurrentPassword = "my-old-password"
                };

                // Act
                var result = await Controller.ChangePassword(request);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }

        public class ChangeAccountInformation : UserControllerTests
        {
            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new ChangeAccountInfoRequestModel
                {
                    Email = "me@me.com",
                    Username = "my-username"
                };
                _mockAuthService.Setup(s => s.ChangeAccountInformation(It.IsAny<ClaimsPrincipal>(), "me@me.com", "my-username", _mockUserManager.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.ChangeAccountInformation(request);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenRequestIsInvalid()
            {
                // Arrange
                var request = new ChangeAccountInfoRequestModel
                {
                    Email = "me@me.com",
                    Username = "my-username"
                };
                var exception = new InvalidAccountInfoUpdateException(new List<string> { "error1", "error2" });
                _mockAuthService.Setup(s => s.ChangeAccountInformation(It.IsAny<ClaimsPrincipal>(), "me@me.com", "my-username", _mockUserManager.Object))
                    .Throws(exception);

                // Act
                var result = await Controller.ChangeAccountInformation(request);
                var body = ((BadRequestObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                body.Should().HaveCount(2);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestSuccessful()
            {
                // Arrange
                var request = new ChangeAccountInfoRequestModel
                {
                    Email = "me@me.com",
                    Username = "my-username"
                };

                // Act
                var result = await Controller.ChangeAccountInformation(request);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
