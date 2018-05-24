// <copyright file="ContactController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

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

    /// <summary>
    /// Controller class for behavior related the Contact Us form.
    /// </summary>
    /// <seealso cref="RPThreadTrackerV3.Controllers.BaseController" />
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactController"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="emailClient">The email client.</param>
        /// <param name="emailBuilder">The email builder.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="mapper">The mapper.</param>
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

        /// <summary>
        /// Processes a request to send a new message to the site's administrators.
        /// </summary>
        /// <param name="model">Information about the message to be sent.</param>
        /// <returns>
        /// HTTP response containing the results of the request.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful processing of message</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
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
