namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Exceptions;
	using Infrastructure.Exceptions.Account;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Models.RequestModels;
	using Models.ViewModels;

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class UserController : BaseController
    {
	    private readonly ILogger<UserController> _logger;
	    private readonly UserManager<IdentityUser> _userManager;
	    private readonly IMapper _mapper;
	    private readonly IAuthService _authService;

	    public UserController(ILogger<UserController> logger, UserManager<IdentityUser> userManager,
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
			    if (user != null)
			    {
				    return Ok(_mapper.Map<UserDto>(user));
			    }
			    return NotFound();

		    }
		    catch (Exception e)
		    {
			    _logger.LogError($"Error retrieving current user: {e.Message}");
		    }
		    return BadRequest("Error retrieving current user.");
	    }

	    [HttpPut]
		[Route("password")]
	    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel request)
	    {
		    try
		    {
			    await _authService.ChangePassword(User, request.CurrentPassword, request.NewPassword, request.ConfirmNewPassword,
				    _userManager);
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
			    return StatusCode(500, new List<string> {"An unknown error occurred."});
		    }
	    }

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
			    return BadRequest(e.Errors);
		    }
		    catch (Exception e)
		    {
				_logger.LogError(e, $"Error requesting account information change for {User.Identity.Name}");
			    return StatusCode(500, new List<string> { "An unknown error occurred." });
			}
	    }
	}
}
