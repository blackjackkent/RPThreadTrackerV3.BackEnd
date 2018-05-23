namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Authentication;
	using System.Threading.Tasks;
	using Infrastructure.Data.Entities;
	using Infrastructure.Exceptions;
	using Infrastructure.Exceptions.Account;
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
	    private readonly IRepository<RefreshToken> _refreshTokenRepository;

	    public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager,
			IConfiguration config, IAuthService authService, IRepository<ProfileSettingsCollection> profileSettingsRepository,
			IEmailClient emailClient, IEmailBuilder emailBuilder, IRepository<RefreshToken> refreshTokenRepository)
		{
			_logger = logger;
			_userManager = userManager;
			_config = config;
			_authService = authService;
			_profileSettingsRepository = profileSettingsRepository;
			_emailClient = emailClient;
			_emailBuilder = emailBuilder;
		    _refreshTokenRepository = refreshTokenRepository;
		}

		[HttpPost("api/auth/token")]
		public async Task<IActionResult> CreateToken([FromBody] LoginRequest model)
		{
		    try
		    {
		        var user = await _authService.GetUserByUsernameOrEmail(model.Username, _userManager);
		        await _authService.ValidatePassword(user, model.Password, _userManager);
		        var jwt = await _authService.GenerateJwt(user, _userManager, _config);
		        var refreshToken = _authService.GenerateRefreshToken(user, _config, _refreshTokenRepository);
		        return Ok(new
		        {
		            token = jwt,
		            refresh_token = refreshToken
		        });
		    }
		    catch (UserNotFoundException)
		    {
		        _logger.LogWarning($"Login failure for {model.Username}. No user exists with this username or email address.");
		        return BadRequest("Invalid username or password.");
		    }
		    catch (InvalidCredentialException)
		    {
		        _logger.LogWarning($"Login failure for {model.Username}. Error validating password.");
		        return BadRequest("Invalid username or password.");
            }
			catch (Exception ex)
			{
				_logger.LogError(default(EventId), ex, $"Error creating JWT: {ex.Message}");
			    return StatusCode(500, "Failed to create JWT.");
            }
	    }

	    [HttpPost("api/auth/refresh")]
	    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
	    {
	        try
	        {
	            var user = _authService.GetUserForRefreshToken(model.RefreshToken, _config, _refreshTokenRepository);
	            var jwt = await _authService.GenerateJwt(user, _userManager, _config);
	            var refreshToken = _authService.GenerateRefreshToken(user, _config, _refreshTokenRepository);
	            return Ok(new
	            {
	                token = jwt,
	                refresh_token = refreshToken
	            });
	        }
	        catch (InvalidRefreshTokenException)
	        {
	            return StatusCode(498);
	        }
	        catch (Exception ex)
	        {
	            _logger.LogError(default(EventId), ex, $"Error refreshing JWT: {ex.Message}");
	            return StatusCode(500, "Failed to create JWT.");
	        }
	    }

	    [HttpPost("api/auth/revoke")]
	    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest model)
	    {
	        try
	        {
	            _authService.RevokeRefreshToken(model.RefreshToken, _config, _refreshTokenRepository);
	            return Ok();
	        }
	        catch (Exception ex)
	        {
	            _logger.LogError(default(EventId), ex, $"Error revoking JWT: {ex.Message}");
	            return StatusCode(500, "Failed to revoke JWT.");
            }
	    }

        [HttpPost("api/auth/register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
		    try
		    {
		        model.AssertIsValid();
		        var user = new IdentityUser
		        {
		            UserName = model.Username,
		            Email = model.Email,
		            SecurityStamp = Guid.NewGuid().ToString()
		        };
		        await _authService.AssertUserInformationDoesNotExist(model.Username, model.Email, _userManager);
		        await _authService.CreateUser(user, model.Password, _userManager);
		        await _authService.AddUserToRole(user, "User", _userManager);
		        _authService.InitProfileSettings(user.Id, _profileSettingsRepository);
		        _logger.LogInformation(3, $"User {model.Username} created a new account with password.");
		        return Ok();
		    }
		    catch (InvalidRegistrationException e)
		    {
		        _logger.LogError(e, $"Error registering user with email {model.Email} and username {model.Username}");
		        return BadRequest(e.Errors);
		    }
		    catch (InvalidAccountInfoUpdateException e)
		    {
		        _logger.LogError(e, $"Error adding {model.Username} to user role.");
		        return BadRequest(e.Errors);
            }
			catch (Exception e)
			{
				_logger.LogError(e, $"Error registering user with email {model.Email} and username {model.Username}");
				return StatusCode(500, new List<string> { "Error creating account. An account with some or all of this information may already exist." });
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
