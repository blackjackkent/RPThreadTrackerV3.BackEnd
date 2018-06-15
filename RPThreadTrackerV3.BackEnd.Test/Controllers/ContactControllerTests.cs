// <copyright file="ContactControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using BackEnd.Controllers;
    using BackEnd.Models.Configuration;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.RequestModels;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "CharacterController")]
    public class ContactControllerTests : ControllerTests<ContactController>
    {
        private readonly AppSettings _mockConfig;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IEmailClient> _mockEmailClient;
        private readonly Mock<IEmailBuilder> _mockEmailBuilder;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;

        public ContactControllerTests()
        {
            var mockLogger = new Mock<ILogger<ContactController>>();
            _mockConfig = new AppSettings();
            var configWrapper = new Mock<IOptions<AppSettings>>();
            configWrapper.SetupGet(c => c.Value).Returns(_mockConfig);
            _mockEmailClient = new Mock<IEmailClient>();
            _mockEmailBuilder = new Mock<IEmailBuilder>();
            _mockAuthService = new Mock<IAuthService>();
            _mockMapper = new Mock<IMapper>();
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            Controller = new ContactController(configWrapper.Object, mockLogger.Object, _mockEmailClient.Object, _mockEmailBuilder.Object, _mockAuthService.Object, _mockUserManager.Object, _mockMapper.Object);
            InitControllerContext();
        }

        public class Post : ContactControllerTests
        {
            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new ContactFormRequestModel { Message = "Test Message" };
                _mockEmailBuilder.Setup(m => m.BuildContactEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), _mockConfig))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Post(request);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
                _mockEmailClient.Verify(c => c.SendEmail(It.IsAny<EmailDto>(), _mockConfig), Times.Never);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestSuccessful()
            {
                // Arrange
                var request = new ContactFormRequestModel { Message = "Test message" };
                var user = new User { Id = "12345" };
                _mockAuthService.Setup(s =>
                        s.GetCurrentUser(It.IsAny<ClaimsPrincipal>(), _mockUserManager.Object, _mockMapper.Object))
                    .Returns(Task.FromResult(user));

                // Act
                var result = await Controller.Post(request);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
