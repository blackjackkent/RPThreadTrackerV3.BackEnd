namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Data.Entities;
	using Interfaces.Data;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Models.DomainModels;
	using Models.ViewModels;

	public class ProfileSettingsController : BaseController
    {
	    private readonly ILogger<ProfileSettingsController> _logger;
	    private readonly UserManager<IdentityUser> _userManager;
	    private readonly IMapper _mapper;
	    private readonly IAuthService _authService;
	    private readonly IRepository<ProfileSettingsCollection> _profileSettingsRepository;

	    public ProfileSettingsController(ILogger<ProfileSettingsController> logger, UserManager<IdentityUser> userManager,
		    IMapper mapper, IAuthService authService, IRepository<ProfileSettingsCollection> profileSettingsRepository)
	    {
		    _logger = logger;
		    _userManager = userManager;
		    _mapper = mapper;
		    _authService = authService;
		    _profileSettingsRepository = profileSettingsRepository;
	    }

	    public async Task<IActionResult> Get()
	    {
			try
			{
				var settings = _authService.GetProfileSettings(UserId, _profileSettingsRepository, _mapper);
				var result = _mapper.Map<ProfileSettingsDto>(settings);
				return Ok(result);
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

	    public async Task<IActionResult> Put(ProfileSettingsDto settings)
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
