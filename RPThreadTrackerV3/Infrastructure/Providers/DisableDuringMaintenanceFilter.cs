namespace RPThreadTrackerV3.Infrastructure.Providers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class DisableDuringMaintenanceFilter : ActionFilterAttribute
    {
        private readonly ILogger<DisableDuringMaintenanceFilter> _logger;
        private readonly IConfiguration _config;

        public DisableDuringMaintenanceFilter(ILogger<DisableDuringMaintenanceFilter> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_config.GetValue<bool>("MaintenanceMode"))
            {
                _logger.LogInformation($"Returning 503 result for maintenance mode: {DateTime.UtcNow}");
                context.Result = new StatusCodeResult(503);
            }
        }
    }
}
