namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Data.Entities;
	using Infrastructure.Exceptions;
	using Infrastructure.Exceptions.Account;
	using Interfaces.Data;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Models.DomainModels;
	using Models.ViewModels;

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class ProfileSettingsController : BaseController
    {
	    private readonly ILogger<ProfileSettingsController> _logger;
	    private readonly IMapper _mapper;
	    private readonly IAuthService _authService;
	    private readonly IRepository<ProfileSettingsCollection> _profileSettingsRepository;

	    public ProfileSettingsController(ILogger<ProfileSettingsController> logger,
		    IMapper mapper, IAuthService authService, IRepository<ProfileSettingsCollection> profileSettingsRepository)
	    {
		    _logger = logger;
		    _mapper = mapper;
		    _authService = authService;
		    _profileSettingsRepository = profileSettingsRepository;
	    }

		[HttpGet]
	    public async Task<IActionResult> Get()
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
		
		[HttpPut]
	    public async Task<IActionResult> Put([FromBody]ProfileSettingsDto settings)
	    {
		    try
		    {
			    var settingsModel = _mapper.Map<ProfileSettings>(settings);
			    _authService.UpdateProfileSettings(settingsModel, UserId, _profileSettingsRepository, _mapper);
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
