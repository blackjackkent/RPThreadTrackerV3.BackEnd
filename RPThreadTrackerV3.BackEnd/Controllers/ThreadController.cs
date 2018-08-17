// <copyright file="ThreadController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using Infrastructure.Data.Entities;
    using Infrastructure.Exceptions.Characters;
    using Infrastructure.Exceptions.Thread;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels;

    /// <summary>
    /// Controller class for behavior related to a user's characters.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="threadService">The thread service.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="characterService">The character service.</param>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="exporterService">The exporter service.</param>
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

        /// <summary>
        /// Processes a request for all threads belonging to the logged-in user.
        /// </summary>
        /// <param name="isArchived">if set to <c>true</c>, includes threads which have been marked as archived.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a <see cref="ThreadDtoCollection" /> object in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of thread information</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ThreadDtoCollection))]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Processes a request to create a new thread for the logged-in user.
        /// </summary>
        /// <param name="thread">Information about the thread to be created.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// the created thread represented as a <see cref="ThreadDto" /> in the
        /// response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful creation of character</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid thread creation request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ThreadDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500)]
		public IActionResult Post([FromBody] ThreadDto thread)
		{
			try
			{
				thread.AssertIsValid();
				_characterService.AssertUserOwnsCharacter(thread.CharacterId, UserId, _characterRepository);
				var model = _mapper.Map<Models.DomainModels.Thread>(thread);
				var createdThread = _threadService.CreateThread(model, _threadRepository, _mapper);
				return Ok(_mapper.Map<ThreadDto>(createdThread));
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

        /// <summary>
        /// Processes a request to update an existing thread belonging to the logged-in user.
        /// </summary>
        /// <param name="threadId">The unique ID of the thread to be updated.</param>
        /// <param name="thread">Information about the thread to be updated.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// the updated thread represented as a <see cref="ThreadDto" /> in the
        /// response body.<para />
        /// <list type="table"><item><term>200 OK</term><description>Response code for successful update of thread information</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid thread update request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPut]
		[Route("{threadId}")]
        [ProducesResponseType(200, Type = typeof(ThreadDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500)]
		public IActionResult Put(int threadId, [FromBody]ThreadDto thread)
		{
			try
			{
				thread.AssertIsValid();
				_threadService.AssertUserOwnsThread(thread.ThreadId, UserId, _threadRepository);
				_characterService.AssertUserOwnsCharacter(thread.CharacterId, UserId, _characterRepository);
				var model = _mapper.Map<Models.DomainModels.Thread>(thread);
				var updatedThread = _threadService.UpdateThread(model, _threadRepository, _mapper);
				return Ok(_mapper.Map<ThreadDto>(updatedThread));
			}
			catch (InvalidThreadException)
			{
				_logger.LogWarning($"User {UserId} attempted to add invalid thread {thread}.");
				return BadRequest("The supplied thread is invalid.");
			}
			catch (ThreadNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to update thread {thread.ThreadId} illegally.");
				return BadRequest("A thread with that ID does not exist.");
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

        /// <summary>
        /// Processes a request to delete an existing thread belonging to the logged-in user.
        /// </summary>
        /// <param name="threadId">The unique ID of the thread to be deleted.</param>
        /// <returns>
        /// HTTP response containing the results of the request.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful deletion of thread</description></item>
        /// <item><term>404 Not Found</term><description>Response code if thread does not exist or does not belong to logged-in user</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpDelete]
		[Route("{threadId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Processes a request to export data about all threads belonging to the current user.
        /// </summary>
        /// <param name="includeHiatused">if set to <c>true</c>, includes data regarding threads for characters who are marked as on hiatus.</param>
        /// <param name="includeArchive">if set to <c>true</c>, includes data regarding threads that are marked as archived.</param>
        /// <returns>
        /// HTTP response containing a binary file stream wrapping the exported data.
        /// </returns>
        [HttpGet]
		[Route("export")]
        [ProducesResponseType(200, Type = typeof(byte[]))]
        [ProducesResponseType(500)]
		public IActionResult Export([FromQuery] bool includeHiatused = false, [FromQuery] bool includeArchive = false)
		{
		    try
		    {
                var characters = _characterService.GetCharacters(UserId, _characterRepository, _mapper, includeHiatused);
		        var threads =
		            _threadService.GetThreadsByCharacter(UserId, includeArchive, includeHiatused, _threadRepository, _mapper);
		        var excelPackage = _exporterService.GetExcelPackage(characters, threads);

		        using (var exportData = new MemoryStream())
		        {
		            excelPackage.Write(exportData);
		            var bytes = exportData.ToArray();
		            var cd = new System.Net.Mime.ContentDisposition
		            {
		                FileName = "Export.xlsx",
		                Inline = false
		            };
		            Response.Headers.Add("Content-Disposition", cd.ToString());
		            Response.Headers.Add("X-Content-Type-Options", "nosniff");
		            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
		    }
		    catch (Exception e)
		    {
                _logger.LogError(e, $"Error exporting threads for User {UserId}: {e.Message}");
		        return StatusCode(500, "An unknown error occurred.");
		    }
		}

        /// <summary>
        /// Processes a request to get all tags for all threads belonging to the current user.
        /// </summary>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a list of thread tags as strings in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of thread tags</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpGet]
	    [Route("tags")]
        [ProducesResponseType(200, Type = typeof(List<string>))]
        [ProducesResponseType(500)]
	    public IActionResult Tags()
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
