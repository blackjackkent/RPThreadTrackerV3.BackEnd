namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Data.Entities;
	using Infrastructure.Exceptions;
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

		[HttpGet]
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

		[HttpPut]
		public IActionResult Put(ThreadDto thread)
		{
			try
			{
				_threadService.AssertUserOwnsThread(thread.ThreadId, UserId, _threadRepository, _mapper);
				var model = _mapper.Map<Models.DomainModels.Thread>(thread);
				_threadService.UpdateThread(model, _threadRepository, _mapper);
				return Ok();
			}
			catch (ThreadNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to update thread {thread.ThreadId} illegally.");
				return NotFound();
			}
			catch (Exception e)
			{
				_logger.LogError($"Error updating thread {thread}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}
	}
}
