// <copyright file="TestController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Infrastructure.Data.Entities;
    using Infrastructure.Enums;
    using Infrastructure.Exceptions.Account;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.Configuration;
    using Models.RequestModels;
    using Models.ViewModels.Auth;

    /// <summary>
    /// Controller class for authentication-related behavior.
    /// </summary>
	[Route("api/[controller]")]
    public class TestController : BaseController
	{
		private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
		{
			_logger = logger;
		}

        /// <summary>
        /// Processes an attempt to authenticate a user's login credentials
        /// and provide them with a JWT and refresh token.
        /// </summary>
        /// <param name="model">Request object containing the user's
        /// login credentials.</param>
        /// <returns>
        /// HTTP response containing information about the operation success or failure and,
        /// if successful, the JWT and refresh token in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful authentication and token generation</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid username or password</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item>
        /// </list>
        /// </returns>
        [HttpGet]
		public async Task<IActionResult> Get()
		{
		    try
		    {
		        return Ok(new List<string> { "test1", "test2" });
		    }
			catch (Exception ex)
			{
				_logger.LogError(default(EventId), ex, $"Error testing: {ex.Message}");
			    return StatusCode(500, "An unknown error occurred.");
            }
	    }
	}
}
