// <copyright file="AuthServiceTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BackEnd.Infrastructure.Exceptions.Account;
    using BackEnd.Infrastructure.Services;
    using FluentAssertions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;

    public class AuthServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly AuthService _authService;
        private Mock<IConfiguration> _mockConfig;

        public AuthServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
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
                _mockConfig.SetupGet(x => x["Tokens:Key"]).Returns("oXHqjGKtAIRBzonnhpmMJuQLoPUd8xH9E1NNlcO5oMhtN");
                _mockConfig.SetupGet(x => x["Tokens:AccessExpireMinutes"]).Returns("12345");
                _mockConfig.SetupGet(x => x["Tokens:Audience"]).Returns("tokenaudience");
                _mockConfig.SetupGet(x => x["Tokens:Issuer"]).Returns("tokenissuer");
                var user = new IdentityUser
                {
                    Id = "12345",
                    UserName = "my-user",
                    Email = "me@me.com"
                };

                // Act
                var result = await _authService.GenerateJwt(user, _mockUserManager.Object, _mockConfig.Object);

                // Assert
                result.Token.Should().NotBe(null);
                result.Expiry.Should().BeGreaterThan(DateTime.UtcNow.Ticks);
            }
        }
    }
}
