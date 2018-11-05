// <copyright file="GlobalExceptionHandlerAttribute.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Providers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Global exception handler which handles and logs all exceptions not caught by other classes.
    /// </summary>
    /// <seealso cref="ExceptionFilterAttribute" />
    [AttributeUsage(AttributeTargets.All)]
    [ExcludeFromCodeCoverage]
    public class GlobalExceptionHandlerAttribute : ExceptionFilterAttribute
    {
	    private readonly ILogger<GlobalExceptionHandlerAttribute> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandlerAttribute"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public GlobalExceptionHandlerAttribute(ILogger<GlobalExceptionHandlerAttribute> logger)
	    {
		    _logger = logger;
	    }

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
		{
			_logger.LogError(context.Exception, $"Unhandled Exception: {context.Exception.Message}");
		}
	}
}
