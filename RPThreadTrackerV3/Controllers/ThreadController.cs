namespace RPThreadTrackerV3.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using AutoMapper;
    using Infrastructure.Data.Entities;
    using Infrastructure.Exceptions;
    using Infrastructure.Exceptions.Thread;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels;
    using RPThreadTrackerV3.Infrastructure.Exceptions.Characters;

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class ThreadController : BaseController
	{
		private readonly ILogger<ThreadController> _logger;
		private readonly IMapper _mapper;
		private readonly IThreadService _threadService;
		private readonly IRepository<Thread> _threadRepository;
		private readonly ICharacterService _characterService;
		private readonly IRepository<Character> _characterRepository;
		private readonly IExporterService _exporterService;

		public ThreadController(
		    ILogger<ThreadController> logger,
		    IMapper mapper,
		    IThreadService threadService,
			IRepository<Thread> threadRepository,
		    ICharacterService characterService,
			IRepository<Character> characterRepository,
		    IExporterService exporterService)
		{
			_logger = logger;
			_mapper = mapper;
			_threadService = threadService;
			_threadRepository = threadRepository;
			_characterService = characterService;
			_characterRepository = characterRepository;
			_exporterService = exporterService;
		}

		[HttpGet]
		public IActionResult Get([FromQuery]bool isArchived = false)
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

		[HttpGet]
		[Route("{threadId}")]
		public IActionResult Get(int threadId)
		{
			try
			{
				var thread = _threadService.GetThread(threadId, UserId, _threadRepository, _mapper);
				var result = _mapper.Map<ThreadDto>(thread);
				return Ok(result);
			}
			catch (ThreadNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to fetch thread {threadId} illegally.");
				return NotFound();
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

		[HttpPost]
		public IActionResult Post([FromBody] ThreadDto thread)
		{
			try
			{
				thread.AssertIsValid();
				_characterService.AssertUserOwnsCharacter(thread.CharacterId, UserId, _characterRepository);
				var model = _mapper.Map<Models.DomainModels.Thread>(thread);
				var createdThread = _threadService.CreateThread(model, UserId, _threadRepository, _mapper);
				return Ok(createdThread);
			}
			catch (InvalidThreadException)
			{
				_logger.LogWarning($"User {UserId} attempted to add invalid thread {thread}.");
				return BadRequest("The supplied thread is invalid.");
			}
			catch (CharacterNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to add thread to character {thread.CharacterId} illegally.");
				return BadRequest("The thread could not be assigned to the given character.");
			}
			catch (Exception e)
			{
				_logger.LogError($"Error creating thread {thread}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

		[HttpPut]
		[Route("{threadId}")]
		public IActionResult Put(int threadId, [FromBody]ThreadDto thread)
		{
			try
			{
				thread.AssertIsValid();
				_threadService.AssertUserOwnsThread(thread.ThreadId, UserId, _threadRepository);
				_characterService.AssertUserOwnsCharacter(thread.CharacterId, UserId, _characterRepository);
				var model = _mapper.Map<Models.DomainModels.Thread>(thread);
				var updatedThread = _threadService.UpdateThread(model, UserId, _threadRepository, _mapper);
				return Ok(updatedThread);
			}
			catch (InvalidThreadException)
			{
				_logger.LogWarning($"User {UserId} attempted to add invalid thread {thread}.");
				return BadRequest("The supplied thread is invalid.");
			}
			catch (ThreadNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to update thread {thread.ThreadId} illegally.");
				return NotFound("A thread with that ID does not exist.");
			}
			catch (CharacterNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to update thread {thread.ThreadId} to character {thread.CharacterId} illegally.");
				return BadRequest("The thread could not be assigned to the given character.");
			}
			catch (Exception e)
			{
				_logger.LogError($"Error updating thread {thread}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

		[HttpDelete]
		[Route("{threadId}")]
		public IActionResult Delete(int threadId)
		{
			try
			{
				_threadService.AssertUserOwnsThread(threadId, UserId, _threadRepository);
				_threadService.DeleteThread(threadId, _threadRepository);
				return Ok();
			}
			catch (ThreadNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to delete thread {threadId} illegally.");
				return NotFound("A thread with that ID does not exist.");
			}
			catch (Exception e)
			{
				_logger.LogError($"Error deleting thread {threadId}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

		[HttpGet]
		[Route("export")]
		public IActionResult Export([FromQuery] bool includeHiatused = false, [FromQuery] bool includeArchive = false)
		{
			var characters = _characterService.GetCharacters(UserId, _characterRepository, _mapper, includeHiatused);
			var threads = _threadService.GetThreadsByCharacter(UserId, includeArchive, includeHiatused, _threadRepository, _mapper);
			var byteArray = _exporterService.GetByteArray(characters, threads);
			var cd = new System.Net.Mime.ContentDisposition
			{
				FileName = "Export.xlsx",
				Inline = false
			};
			Response.Headers.Add("Content-Disposition", cd.ToString());
			Response.Headers.Add("X-Content-Type-Options", "nosniff");
			return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
	    }

	    [HttpGet]
	    [Route("tags")]
	    public IActionResult Tags([FromQuery] bool includeArchive = false)
	    {
	        try
	        {
	            var tags = _threadService.GetAllTags(UserId, _threadRepository, _mapper);
	            return Ok(tags);
	        }
	        catch (Exception e)
	        {
	            _logger.LogError(e, $"Error retrieving tags for user: {e.Message}");
	            return StatusCode(500, "An unknown error occurred.");
            }
	    }
    }
}
