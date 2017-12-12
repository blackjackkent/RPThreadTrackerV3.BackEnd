namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.IdentityModel.Tokens.Jwt;
	using System.Threading.Tasks;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Models.RequestModels;

	public class AuthController : Controller
	{
		private readonly ILogger<AuthController> _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IConfigurationRoot _config;
		private readonly IAuthService _authService;

		public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager, 
		IConfigurationRoot config, IAuthService authService)
		{
			_logger = logger;
			_userManager = userManager;
			_config = config;
			_authService = authService;
		}

		[HttpPost("api/auth/token")]
		public async Task<IActionResult> CreateToken([FromBody] LoginRequest model)
		{
			try
			{
				var user = await _authService.GetUserByUsernameOrEmail(model.Username, _userManager);
				if (user == null)
				{
					_logger.LogWarning($"Login failure for {model.Username}. No user exists with this username or email address.");
					return BadRequest("Invalid username or password.");
				}
				var verificationResult = await _userManager.CheckPasswordAsync(user, model.Password);
				if (!verificationResult)
				{
					_logger.LogWarning($"Login failure for {model.Username}. Error validating password.");
					return BadRequest("Invalid username or password.");
				}
				var jwt = await _authService.GenerateJwt(user, _config["Tokens:Key"], _config["Tokens:Issuer"], _config["Tokens:Audience"], _userManager);
				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(jwt),
					expiration = jwt.ValidTo
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(default(EventId), ex, $"Error creating JWT: {ex.Message}");
			}
			return BadRequest("Failed to create JWT.");
		}

		[HttpPost("api/auth/register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser { UserName = model.Email, Email = model.Email };
				try
				{
					var result = await _userManager.CreateAsync(user, model.Password);
					var roleResult = await _userManager.AddToRoleAsync(user, "User");
					if (result.Succeeded && roleResult.Succeeded)
					{
						_logger.LogInformation(3, "User created a new account with password.");
						return Ok();
					}
					return BadRequest(result);
				}
				catch (Exception e)
				{
					return BadRequest(e);
				}
			}
			return BadRequest();
		}
	}
}
