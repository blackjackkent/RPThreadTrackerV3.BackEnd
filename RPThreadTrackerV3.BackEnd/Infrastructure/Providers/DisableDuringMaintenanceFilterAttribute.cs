// <copyright file="DisableDuringMaintenanceFilterAttribute.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Providers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.Configuration;

    /// <summary>
    /// Filter attribute which enforces 503 responses from all controllers when
    /// the application config indicates the site is in maintenance mode.
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    [AttributeUsage(AttributeTargets.All)]
    public class DisableDuringMaintenanceFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<DisableDuringMaintenanceFilterAttribute> _logger;
        private readonly AppSettings _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableDuringMaintenanceFilterAttribute"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="config">The configuration.</param>
        public DisableDuringMaintenanceFilterAttribute(ILogger<DisableDuringMaintenanceFilterAttribute> logger, IOptions<AppSettings> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_config.Maintenance.MaintenanceMode)
            {
                _logger.LogInformation($"Returning 503 result for maintenance mode: {DateTime.UtcNow}");
                context.Result = new StatusCodeResult(503);
            }
        }
    }
}
