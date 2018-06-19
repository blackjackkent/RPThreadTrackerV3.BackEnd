// <copyright file="UserController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Exceptions.Account;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.RequestModels;
    using Models.ViewModels;

    /// <summary>
    /// Controller class for behavior related to a user.
    /// </summary>
    /// <seealso cref="BaseController" />
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class UserController : BaseController
    {
	    private readonly ILogger<UserController> _logger;
	    private readonly UserManager<IdentityUser> _userManager;
	    private readonly IMapper _mapper;
	    private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="authService">The authentication service.</param>
        public UserController(
	        ILogger<UserController> logger,
	        UserManager<IdentityUser> userManager,
		    IMapper mapper,
	        IAuthService authService)
	    {
		    _logger = logger;
		    _userManager = userManager;
		    _mapper = mapper;
		    _authService = authService;
	    }

        /// <summary>
        /// Processes a request for the currently logged-in user.
        /// </summary>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a <see cref="UserDto"/> object in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of user information</description></item>
        /// <item><term>404 Not Found</term><description>Response code when information for the current user could not be found</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item>
        /// </list>
        /// </returns>
        [HttpGet]
	    public async Task<IActionResult> Get()
	    {
	        try
	        {
	            var claimsUser = User;
	            var user = await _authService.GetCurrentUser(claimsUser, _userManager, _mapper);
	            return Ok(_mapper.Map<UserDto>(user));
	        }
	        catch (UserNotFoundException)
	        {
	            return NotFound();
	        }
		    catch (Exception e)
		    {
			    _logger.LogError($"Error retrieving current user: {e.Message}");
		        return StatusCode(500, "Error retrieving current user.");
            }
	    }

        /// <summary>
        /// Processes a request to update the password of the current user.
        /// </summary>
        /// <param name="request">Request object containing the user's old and new password information.</param>
        /// <returns>
        /// HTTP response containing the results of the request<para />
        /// <list type="table"><item><term>200 OK</term><description>Response code for successful update of password</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid password update request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPut]
		[Route("password")]
	    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel request)
	    {
		    try
		    {
			    await _authService.ChangePassword(User, request.CurrentPassword, request.NewPassword, request.ConfirmNewPassword, _userManager);
			    return Ok();
		    }
		    catch (InvalidChangePasswordException e)
		    {
			    _logger.LogWarning(e, $"Error resetting password for {User.Identity.Name}: {e.Errors}");
			    return BadRequest(e.Errors);
		    }
		    catch (Exception e)
		    {
			    _logger.LogError(e, $"Error requesting password reset for {User.Identity.Name}");
			    return StatusCode(500, new List<string> { "An unknown error occurred." });
		    }
	    }

        /// <summary>
        /// Processes a request to update the current user's account information.
        /// </summary>
        /// <param name="request">Request object containing the user's updated account information.</param>
        /// <returns>
        /// HTTP response containing the results of the request<para />
        /// <list type="table"><item><term>200 OK</term><description>Response code for successful update of account information</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid account information update request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPut]
	    [Route("accountinfo")]
	    public async Task<IActionResult> ChangeAccountInformation([FromBody] ChangeAccountInfoRequestModel request)
	    {
		    try
		    {
			    await _authService.ChangeAccountInformation(User, request.Email, request.Username, _userManager);
			    return Ok();
		    }
		    catch (InvalidAccountInfoUpdateException e)
		    {
			    _logger.LogWarning(e, $"Error updating account info for {User.Identity.Name}: {e.Errors}");
			    return BadRequest(new List<string> { "Error updating account. An account with some or all of this information may already exist." });
		    }
		    catch (Exception e)
		    {
				_logger.LogError(e, $"Error requesting account information change for {User.Identity.Name}");
			    return StatusCode(500, new List<string> { "An unknown error occurred." });
			}
	    }
	}
}
