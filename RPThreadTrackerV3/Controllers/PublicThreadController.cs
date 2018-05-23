namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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

    [Route("api/[controller]")]
	public class PublicThreadController : BaseController
    {
	    private readonly ILogger<ThreadController> _logger;
	    private readonly IMapper _mapper;
	    private readonly IThreadService _threadService;
	    private readonly IRepository<Thread> _threadRepository;
	    private readonly IPublicViewService _publicViewService;
	    private readonly IDocumentRepository<PublicView> _publicViewRepository;

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
