namespace RPThreadTrackerV3.Infrastructure.Providers
{
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.Extensions.Logging;

	public class GlobalExceptionHandler : ExceptionFilterAttribute
    {
	    private readonly ILogger<GlobalExceptionHandler> _logger;

	    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	    {
		    _logger = logger;
	    }
		public override void OnException(ExceptionContext context)
		{
			_logger.LogError(default(EventId), context.Exception, $"Unhandled Exception: {context.Exception.Message}");
		}
	}
}
