namespace RPThreadTrackerV3.Infrastructure.Providers
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.Extensions.Logging;

    [AttributeUsage(AttributeTargets.All)]
	public class GlobalExceptionHandlerAttribute : ExceptionFilterAttribute
    {
	    private readonly ILogger<GlobalExceptionHandlerAttribute> _logger;

	    public GlobalExceptionHandlerAttribute(ILogger<GlobalExceptionHandlerAttribute> logger)
	    {
		    _logger = logger;
	    }
		public override void OnException(ExceptionContext context)
		{
			_logger.LogError(default(EventId), context.Exception, $"Unhandled Exception: {context.Exception.Message}");
		}
	}
}
