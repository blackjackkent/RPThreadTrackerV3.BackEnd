namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Data.Entities;
	using Interfaces.Data;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Models.ViewModels;

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class ThreadController : BaseController
	{
		private readonly ILogger<ThreadController> _logger;
		private readonly IMapper _mapper;
		private readonly IThreadService _threadService;
		private readonly IRepository<Thread> _threadRepository;

		public ThreadController(ILogger<ThreadController> logger, IMapper mapper, IThreadService threadService, IRepository<Thread> threadRepository)
		{
			_logger = logger;
			_mapper = mapper;
			_threadService = threadService;
			_threadRepository = threadRepository;
		}

		public IActionResult Get(bool isArchived = false)
		{
			try
			{
				var threads = _threadService.GetThreads(UserId, isArchived, _threadRepository, _mapper);
				var result = threads.Select(_mapper.Map<ThreadDto>).ToList();
				var response = new ThreadDtoCollection(result);
				return Ok(response);
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				return StatusCode(500, "An unknown error occurred.");
			}
		}
	}
}
