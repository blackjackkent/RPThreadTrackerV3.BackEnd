// <copyright file="ProfileSettingsControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using AutoMapper;
    using BackEnd.Controllers;
    using BackEnd.Infrastructure.Data.Entities;
    using BackEnd.Infrastructure.Exceptions.Account;
    using BackEnd.Models.Configuration;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "ProfileSettingsController")]
    public class ProfileSettingsControllerTests : ControllerTests<ProfileSettingsController>
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IRepository<ProfileSettingsCollection>> _mockProfileSettingsRepository;
        private readonly Mock<IMapper> _mockMapper;

        public ProfileSettingsControllerTests()
        {
            var mockLogger = new Mock<ILogger<ProfileSettingsController>>();
            _mockMapper = new Mock<IMapper>();

            _mockMapper.Setup(m => m.Map<ProfileSettingsDto>(It.IsAny<ProfileSettings>()))
                .Returns((ProfileSettings model) => new ProfileSettingsDto
                {
                    UserId = model.UserId,
                    ShowDashboardThreadDistribution = model.ShowDashboardThreadDistribution,
                    LastNewsReadDate = model.LastNewsReadDate,
                    SettingsId = model.SettingsId
                });
            _mockMapper.Setup(m => m.Map<ProfileSettings>(It.IsAny<ProfileSettingsDto>()))
                .Returns((ProfileSettingsDto dto) => new ProfileSettings
                {
                    UserId = dto.UserId,
                    ShowDashboardThreadDistribution = dto.ShowDashboardThreadDistribution,
                    LastNewsReadDate = dto.LastNewsReadDate,
                    SettingsId = dto.SettingsId
                });
            _mockAuthService = new Mock<IAuthService>();
            _mockProfileSettingsRepository = new Mock<IRepository<ProfileSettingsCollection>>();
            var mockConfig = new AppSettings();
            var configWrapper = new Mock<IOptions<AppSettings>>();
            configWrapper.SetupGet(c => c.Value).Returns(mockConfig);
            Controller = new ProfileSettingsController(mockLogger.Object, _mockMapper.Object, _mockAuthService.Object, _mockProfileSettingsRepository.Object);
            InitControllerContext();
        }

        public class GetProfileSettings : ProfileSettingsControllerTests
        {
            [Fact]
            public void ReturnsNotFoundWhenProfileSettingsNotFoundForUser()
            {
                // Arrange
                _mockAuthService.Setup(s => s.GetProfileSettings("12345", _mockProfileSettingsRepository.Object, _mockMapper.Object))
                    .Throws<ProfileSettingsNotFoundException>();

                // Act
                var result = Controller.Get();

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockAuthService.Setup(s => s.GetProfileSettings("12345", _mockProfileSettingsRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Get();

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWithProfileSettingsWhenRequestSuccessful()
            {
                // Arrange
                var settings = new ProfileSettings
                {
                    UserId = "12345",
                    SettingsId = 54321
                };
                _mockAuthService.Setup(s => s.GetProfileSettings("12345", _mockProfileSettingsRepository.Object, _mockMapper.Object))
                    .Returns(settings);

                // Act
                var result = Controller.Get();
                var body = ((OkObjectResult)result).Value as ProfileSettingsDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.UserId.Should().Be("12345");
                body.SettingsId.Should().Be(54321);
            }
        }

        public class Put : ProfileSettingsControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new ProfileSettingsDto
                {
                    UserId = "12345",
                    SettingsId = 54321
                };
                _mockAuthService.Setup(s => s.UpdateProfileSettings(It.IsAny<ProfileSettings>(), _mockProfileSettingsRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Put(request);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                var request = new ProfileSettingsDto
                {
                    UserId = "12345",
                    SettingsId = 54321
                };

                // Act
                var result = Controller.Put(request);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
