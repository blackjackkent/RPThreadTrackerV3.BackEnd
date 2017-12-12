using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPThreadTrackerV3.Infrastructure.Providers
{
	using System.Net;
	using Microsoft.AspNetCore.Diagnostics;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
