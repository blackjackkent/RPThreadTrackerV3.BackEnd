// <copyright file="BaseControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System.Security.Claims;
    using BackEnd.Controllers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "BaseController")]
    public class BaseControllerTests : ControllerTests<BaseController>
    {
        private readonly MockChildController _childController;

        public BaseControllerTests()
        {
            _childController = new MockChildController();
        }

        public class Properties : BaseControllerTests
        {
            [Fact]
            public void UserIdReturnsClaimsPrincipalUserId()
            {
                // Arrange
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "12345")
                }));
                _childController.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };

                // Act
                var userId = _childController.RetrieveUserId();

                // Assert
                userId.Should().Be("12345");
            }
        }
    }
}
