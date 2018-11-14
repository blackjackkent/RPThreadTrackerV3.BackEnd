// <copyright file="PublicThreadController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Data.Documents;
    using Infrastructure.Data.Entities;
    using Infrastructure.Exceptions.PublicViews;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels;
    using Models.ViewModels.PublicViews;
    using Newtonsoft.Json;

    /// <summary>
    /// Controller class for behavior related to public thread collections.
    /// </summary>
    [Route("api/[controller]")]
	public class PublicThreadController : BaseController
    {
	    private readonly ILogger<PublicThreadController> _logger;
	    private readonly IMapper _mapper;
	    private readonly IThreadService _threadService;
	    private readonly IRepository<Thread> _threadRepository;
	    private readonly IPublicViewService _publicViewService;
	    private readonly IDocumentRepository<PublicView> _publicViewRepository;
        private readonly ICharacterService _characterService;
        private readonly IRepository<Character> _characterRepository;
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicThreadController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="threadService">The thread service.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="publicViewService">The public view service.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <param name="characterService">The character service.</param>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="authService">The auth service.</param>
        /// <param name="userManager">The user manager.</param>
        public PublicThreadController(
		    ILogger<PublicThreadController> logger,
		    IMapper mapper,
		    IThreadService threadService,
		    IRepository<Thread> threadRepository,
		    IPublicViewService publicViewService,
		    IDocumentRepository<PublicView> publicViewRepository,
		    ICharacterService characterService,
		    IRepository<Character> characterRepository,
		    IAuthService authService,
		    UserManager<IdentityUser> userManager)
	    {
		    _logger = logger;
		    _mapper = mapper;
		    _threadService = threadService;
		    _threadRepository = threadRepository;
		    _publicViewService = publicViewService;
		    _publicViewRepository = publicViewRepository;
	        _characterService = characterService;
	        _characterRepository = characterRepository;
	        _authService = authService;
	        _userManager = userManager;
	    }

        /// <summary>
        /// Processes a request for all threads associated with a particular public view.
        /// </summary>
        /// <param name="username">Username of the user to whom the slug belongs.</param>
        /// <param name="slug">Unique string identifier for a public view.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a <see cref="PublicThreadDtoCollection" /> object in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of public threads information</description></item>
        /// <item><term>404 Not Found</term><description>Response code if no public view matches the slug</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpGet]
	    [Route("{username}/{slug}")]
        [ProducesResponseType(200, Type = typeof(PublicThreadDtoCollection))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
	    public async Task<IActionResult> Get(string username, string slug)
	    {
		    try
		    {
		        _logger.LogInformation($"Received request to get public threads for view with slug {slug} belonging to user {username}");
                var user = await _authService.GetUserByUsernameOrEmail(username, _userManager);
			    var view = await _publicViewService.GetViewBySlugAndUserId(slug, user.Id, _publicViewRepository, _mapper);
			    var viewDto = _mapper.Map<PublicViewDto>(view);
			    var threads = _threadService.GetThreadsForView(view, _threadRepository, _mapper);
			    var dtos = _mapper.Map<List<ThreadDto>>(threads);
				var collection = new PublicThreadDtoCollection(dtos, viewDto);
		        _logger.LogInformation($"Processed request to get public threads for view with slug {slug} belonging to user {username}. Found {collection.Threads.Count} threads.");
                return Ok(collection);
		    }
		    catch (PublicViewNotFoundException)
		    {
				_logger.LogWarning($"User {UserId} attempted to fetch nonexistant public view {slug} with username {username}.");
			    return NotFound();
			}
		    catch (Exception e)
		    {
			    _logger.LogError(e, $"Error retrieving threads for public view with slug {slug} and username {username}: {e.Message}");
			    return StatusCode(500, "An unexpected error occurred.");
		    }
	    }

        /// <summary>
        /// Processes a request for all threads associated with a legacy public view.
        /// </summary>
        /// <param name="model">Object representation of a legacy public view.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a <see cref="PublicThreadDtoCollection" /> object in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of public threads information</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [Obsolete("No longer relevant after legacy public views are disabled.")]
        [HttpPost]
        [Route("")]
        [ProducesResponseType(200, Type = typeof(PublicThreadDtoCollection))]
        [ProducesResponseType(500)]
        public IActionResult Post([FromBody] LegacyPublicViewDto model)
        {
            try
            {
                _logger.LogInformation($"Received request to get public threads for legacy view with slug {model.Slug} belonging to user {model.UserId}. Request body: {JsonConvert.SerializeObject(model)}");
                var characters = _characterService.GetCharacters(model.UserId, _characterRepository, _mapper, false);
                var view = _publicViewService.GetViewFromLegacyDto(model, characters);
                var threads = _threadService.GetThreadsForView(view, _threadRepository, _mapper);
                var dtos = _mapper.Map<List<ThreadDto>>(threads);
                var viewDto = _mapper.Map<PublicViewDto>(view);
                var collection = new PublicThreadDtoCollection(dtos, viewDto);
                _logger.LogInformation($"Processed request to get public threads for legacy view with slug {model.Slug} belonging to user {model.UserId}");
                return Ok(collection);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error retrieving threads for legacy public view: {JsonConvert.SerializeObject(model)}: {e.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
