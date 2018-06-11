// <copyright file="DisableDuringMaintenanceFilterAttributeTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Providers
{
    using System.Collections.Generic;
    using BackEnd.Infrastructure.Providers;
    using BackEnd.Infrastructure.Services;
    using BackEnd.Models.Configuration;
    using FluentAssertions;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [Trait("Class", "DisableDuringMaintenanceFilterAttribute")]
    public class DisableDuringMaintenanceFilterAttributeTests
    {
        private readonly Mock<ILogger<DisableDuringMaintenanceFilterAttribute>> _mockLogger;
        private readonly AppSettings _mockConfig;
        private readonly DisableDuringMaintenanceFilterAttribute _filter;
        private readonly ActionExecutingContext _mockContext;

        public DisableDuringMaintenanceFilterAttributeTests()
        {
            _mockLogger = new Mock<ILogger<DisableDuringMaintenanceFilterAttribute>>();
            _mockConfig = new AppSettings();
            _mockContext = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object)
            {
                Result = new OkResult()
            };
            _filter = new DisableDuringMaintenanceFilterAttribute(_mockLogger.Object, _mockConfig);
        }

        public class OnActionExecuting : DisableDuringMaintenanceFilterAttributeTests
        {
            [Fact]
            public void Triggers503StatusCodeWhenMaintenanceModeEnabledInConfig()
            {
                // Arrange
                _mockConfig.Maintenance.MaintenanceMode = true;

                // Act
                _filter.OnActionExecuting(_mockContext);

                // Assert
                ((StatusCodeResult)_mockContext.Result).StatusCode.Should().Be(503);
            }

            [Fact]
            public void DoesNotTrigger503StatusCodeWhenMaintenanceModeNotEnabledInConfig()
            {
                // Arrange
                _mockConfig.Maintenance.MaintenanceMode = false;

                // Act
                _filter.OnActionExecuting(_mockContext);

                // Assert
                ((StatusCodeResult)_mockContext.Result).StatusCode.Should().Be(200);
            }
        }
    }
}
