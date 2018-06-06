// <copyright file="ProfileSettingsController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using AutoMapper;
    using Infrastructure.Data.Entities;
    using Infrastructure.Exceptions.Account;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.DomainModels;
    using Models.ViewModels;

    /// <summary>
    /// Controller class for behavior relating to user profile settings.
    /// </summary>
    /// <seealso cref="BaseController" />
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class ProfileSettingsController : BaseController
    {
	    private readonly ILogger<ProfileSettingsController> _logger;
	    private readonly IMapper _mapper;
	    private readonly IAuthService _authService;
	    private readonly IRepository<ProfileSettingsCollection> _profileSettingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSettingsController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="profileSettingsRepository">The profile settings repository.</param>
        public ProfileSettingsController(ILogger<ProfileSettingsController> logger, IMapper mapper, IAuthService authService, IRepository<ProfileSettingsCollection> profileSettingsRepository)
	    {
		    _logger = logger;
		    _mapper = mapper;
		    _authService = authService;
		    _profileSettingsRepository = profileSettingsRepository;
	    }

        /// <summary>
        /// Processes a request for the currently logged-in user's profile settings.
        /// </summary>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a <see cref="ProfileSettingsDto"/> object in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of profile settings information</description></item>
        /// <item><term>404 Not Found</term><description>Response code if the user's profile information could not be found</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item>
        /// </list>
        /// </returns>
        [HttpGet]
	    public IActionResult Get()
	    {
		    try
		    {
			    var settings = _authService.GetProfileSettings(UserId, _profileSettingsRepository, _mapper);
			    var result = _mapper.Map<ProfileSettingsDto>(settings);
			    return Ok(result);
		    }
		    catch (ProfileSettingsNotFoundException e)
		    {
			    _logger.LogError(e, $"A profile settings object was not found for user {UserId}");
			    return NotFound();
		    }
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

        /// <summary>
        /// Processes a request to update the currently logged-in user's profile settings.
        /// </summary>
        /// <param name="settings">Information about the settings to be updated.</param>
        /// <returns>
        /// HTTP response containing the results of the request.<para /><list type="table"><item><term>200 OK</term><description>Response code for successful retrieval of profile settings information</description></item><item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
		[HttpPut]
	    public IActionResult Put([FromBody]ProfileSettingsDto settings)
	    {
		    try
		    {
			    var settingsModel = _mapper.Map<ProfileSettings>(settings);
			    _authService.UpdateProfileSettings(settingsModel, _profileSettingsRepository, _mapper);
			    return Ok();
		    }
		    catch (Exception e)
		    {
				_logger.LogError(e, e.Message);
			    return StatusCode(500, "An unknown error occurred.");
			}
	    }
	}
}
