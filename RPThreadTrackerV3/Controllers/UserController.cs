namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Threading.Tasks;
	using AutoMapper;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Models.ViewModels;
	[Authorize]
	[Route("api/[controller]")]
	public class UserController : BaseController
    {
	    private readonly ILogger<AuthController> _logger;
	    private readonly UserManager<IdentityUser> _userManager;
	    private readonly IMapper _mapper;
	    private readonly IAuthService _authService;

	    public UserController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager,
		    IMapper mapper, IAuthService authService)
	    {
		    _logger = logger;
		    _userManager = userManager;
		    _mapper = mapper;
		    _authService = authService;
	    }
		// GET api/values
		[HttpGet]
	    public async Task<IActionResult> Get()
	    {
		    try
		    {
			    var claimsUser = User;
			    var user = await _authService.GetCurrentUser(claimsUser, _userManager, _mapper);
			    if (user == null)
			    {
					return NotFound(); ;
			    }
			    return Ok(_mapper.Map<UserDto>(user));

		    }
		    catch (Exception e)
		    {
			    _logger.LogError($"Error retrieving current user: {e.Message}");
		    }
		    return BadRequest("Error retrieving current user.");
	    }
	}
}
