namespace RPThreadTrackerV3.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Models.RequestModels;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ContactController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ContactController> _logger;
        private readonly IEmailClient _emailClient;
        private readonly IEmailBuilder _emailBuilder;
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public ContactController(IConfiguration config, ILogger<ContactController> logger, IEmailClient emailClient, IEmailBuilder emailBuilder, IAuthService authService, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _config = config;
            _logger = logger;
            _emailClient = emailClient;
            _emailBuilder = emailBuilder;
            _authService = authService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactFormRequestModel model)
        {
            try
            {
                var user = await _authService.GetCurrentUser(User, _userManager, _mapper);
                var email = _emailBuilder.BuildContactEmail(user.Email, user.UserName, model.Message, _config);
                await _emailClient.SendEmail(email, _config);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error sending contact form. Message was: {model.Message}. {e}");
                return StatusCode(500, "An unknown error occurred.");
            }
        }
    }
}
