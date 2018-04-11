namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Linq;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Data.Entities;
	using Infrastructure.Exceptions;
	using Interfaces.Data;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Models.RequestModels;

	public class AuthController : BaseController
	{
		private readonly ILogger<AuthController> _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IConfiguration _config;
		private readonly IAuthService _authService;
		private readonly IRepository<ProfileSettingsCollection> _profileSettingsRepository;
		private readonly IEmailClient _emailClient;
		private readonly IEmailBuilder _emailBuilder;

		public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager,
			IConfiguration config, IAuthService authService, IRepository<ProfileSettingsCollection> profileSettingsRepository,
			IEmailClient emailClient, IEmailBuilder emailBuilder)
		{
			_logger = logger;
			_userManager = userManager;
			_config = config;
			_authService = authService;
			_profileSettingsRepository = profileSettingsRepository;
			_emailClient = emailClient;
			_emailBuilder = emailBuilder;
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
				var jwt = await _authService.GenerateJwt(user, _config["Tokens:Key"], _config["Tokens:Issuer"],
					_config["Tokens:Audience"], _userManager);
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
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var user = new IdentityUser
			{
				UserName = model.Username,
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			try
			{
				var result = await _userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
				{
					return BadRequest(result.Errors.Select(e => e.Description));
				}
				var roleResult = await _userManager.AddToRoleAsync(user, "User");
				if (!roleResult.Succeeded)
				{
					return BadRequest(roleResult.Errors);
				}
				_authService.InitProfileSettings(user.Id, _profileSettingsRepository);
				_logger.LogInformation(3, "User created a new account with password.");
				return Ok();
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error registering user with username {model.Email}");
				return StatusCode(500, "An unknown error occurred.");
			}
		}

		[HttpPost("api/auth/forgotpassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var email = _emailBuilder.BuildForgotPasswordEmail(user, _config["CorsUrl"], code, _config);
				await _emailClient.SendEmail(email, _config);
				return Ok();
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error requesting password reset for {model.Email}");
				return StatusCode(500, "An unknown error occurred.");
			}
		}

		[HttpPost("api/auth/resetpassword")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
		{
			try
			{
				if (!model.NewPassword.Equals(model.ConfirmNewPassword))
				{
					throw new InvalidPasswordResetException(new List<string> {"Passwords do not match."});
				}
				await _authService.ResetPassword(model.Email, model.Code, model.NewPassword, _userManager);
				return Ok();
			}
			catch (InvalidPasswordResetException e)
			{
				_logger.LogError(e, $"Error resetting password for {model.Email}: {e.Errors}");
				return BadRequest(e.Errors);
			}
			catch (UserNotFoundException e)
			{
				_logger.LogError(e, $"Error resetting password for {model.Email}. User does not exist.");
				return BadRequest("This reset token is invalid. Please request a new password reset link.");
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error requesting password reset for {model.Email}");
				return StatusCode(500, "An unknown error occurred.");
			}
		}
	}
}
