// <copyright file="PublicThreadController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Controllers
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
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Models.ViewModels;
	using Models.ViewModels.PublicViews;

    /// <summary>
    /// Controller class for behavior related to public thread collections.
    /// </summary>
    /// <seealso cref="RPThreadTrackerV3.Controllers.BaseController" />
    [Route("api/[controller]")]
	public class PublicThreadController : BaseController
    {
	    private readonly ILogger<ThreadController> _logger;
	    private readonly IMapper _mapper;
	    private readonly IThreadService _threadService;
	    private readonly IRepository<Thread> _threadRepository;
	    private readonly IPublicViewService _publicViewService;
	    private readonly IDocumentRepository<PublicView> _publicViewRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicThreadController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="threadService">The thread service.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="publicViewService">The public view service.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        public PublicThreadController(
		    ILogger<ThreadController> logger,
		    IMapper mapper,
		    IThreadService threadService,
		    IRepository<Thread> threadRepository,
		    IPublicViewService publicViewService,
		    IDocumentRepository<PublicView> publicViewRepository)
	    {
		    _logger = logger;
		    _mapper = mapper;
		    _threadService = threadService;
		    _threadRepository = threadRepository;
		    _publicViewService = publicViewService;
		    _publicViewRepository = publicViewRepository;
	    }

        /// <summary>
        /// Processes a request for all threads associated with a particular public view.
        /// </summary>
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
	    [Route("{slug}")]
	    public async Task<IActionResult> Get(string slug)
	    {
		    try
		    {
			    var view = await _publicViewService.GetViewBySlug(slug, _publicViewRepository, _mapper);
			    var viewDto = _mapper.Map<PublicViewDto>(view);
			    var threads = _threadService.GetThreadsForView(view, _threadRepository, _mapper);
			    var dtos = _mapper.Map<List<ThreadDto>>(threads);
				var collection = new PublicThreadDtoCollection(dtos, viewDto);
			    return Ok(collection);
		    }
		    catch (PublicViewNotFoundException)
		    {
				_logger.LogWarning($"User {UserId} attempted to fetch nonexistant public view {slug}.");
			    return NotFound();
			}
		    catch (Exception e)
		    {
			    _logger.LogError(e, $"Error retrieving threads for public view with slug {slug}: {e.Message}");
			    return StatusCode(500, "An unknown error occurred.");
		    }
	    }
    }
}
