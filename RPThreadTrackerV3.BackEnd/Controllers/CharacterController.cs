// <copyright file="CharacterController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Infrastructure.Data.Entities;
    using Infrastructure.Exceptions.Characters;
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
	public class CharacterController : BaseController
	{
		private readonly ILogger<CharacterController> _logger;
		private readonly IMapper _mapper;
		private readonly ICharacterService _characterService;
		private readonly IRepository<Character> _characterRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="characterService">The character service.</param>
        /// <param name="characterRepository">The character repository.</param>
        public CharacterController(
		    ILogger<CharacterController> logger,
		    IMapper mapper,
		    ICharacterService characterService,
		    IRepository<Character> characterRepository)
		{
			_logger = logger;
			_mapper = mapper;
			_characterService = characterService;
			_characterRepository = characterRepository;
		}

	    /// <summary>
	    /// Processes a request for all characters belonging to the logged-in user.
	    /// </summary>
	    /// <returns>
	    /// HTTP response containing the results of the request and, if successful,
	    /// a list of <see cref="CharacterDto" /> objects in the response body.<para />
	    /// <list type="table">
	    /// <item><term>200 OK</term><description>Response code for successful retrieval of character information</description></item>
	    /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item>
	    /// </list>
	    /// </returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CharacterDto>))]
        [ProducesResponseType(500)]
		public IActionResult Get()
		{
			try
			{
				var characters = _characterService.GetCharacters(UserId, _characterRepository, _mapper);
				var result = characters.Select(_mapper.Map<CharacterDto>).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

        /// <summary>
        /// Processes a request to create a new character for the logged-in user.
        /// </summary>
        /// <param name="character">Information about the character to be created.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// the created character represented as a <see cref="CharacterDto" /> in the
        /// response body.<para /><list type="table"><item><term>200 OK</term><description>Response code for successful creation of character</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid character creation request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(CharacterDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500)]
		public IActionResult Post([FromBody] CharacterDto character)
		{
			try
			{
				character.AssertIsValid();
				character.UserId = UserId;
				var model = _mapper.Map<Models.DomainModels.Character>(character);
				var createdCharacter = _characterService.CreateCharacter(model, _characterRepository, _mapper);
				return Ok(_mapper.Map<CharacterDto>(createdCharacter));
			}
			catch (InvalidCharacterException)
			{
				_logger.LogWarning($"User {UserId} attempted to add invalid character {character}.");
				return BadRequest("The supplied character is invalid.");
			}
			catch (Exception e)
			{
				_logger.LogError($"Error creating characted {character}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

        /// <summary>
        /// Processes a request to update an existing character belonging to the logged-in user.
        /// </summary>
        /// <param name="characterId">The unique ID of the character to be updated.</param>
        /// <param name="character">Information about the character to be updated.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// the updated character represented as a <see cref="CharacterDto" /> in the
        /// response body.<para />
        /// <list type="table"><item><term>200 OK</term><description>Response code for successful update of character information</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid character update request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPut]
		[Route("{characterId}")]
        [ProducesResponseType(200, Type = typeof(CharacterDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500)]
		public IActionResult Put(int characterId, [FromBody]CharacterDto character)
		{
			try
			{
				character.AssertIsValid();
				_characterService.AssertUserOwnsCharacter(characterId, UserId, _characterRepository);
				var model = _mapper.Map<Models.DomainModels.Character>(character);
				var updatedCharacter = _characterService.UpdateCharacter(model, _characterRepository, _mapper);
				return Ok(_mapper.Map<CharacterDto>(updatedCharacter));
			}
			catch (InvalidCharacterException)
			{
				_logger.LogWarning($"User {UserId} attempted to update invalid character {character}.");
				return BadRequest("The supplied character is invalid.");
			}
			catch (CharacterNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to update character {character.CharacterId} illegally.");
				return BadRequest("You do not have permission to update this character.");
			}
			catch (Exception e)
			{
				_logger.LogError($"Error updating character {character}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}

        /// <summary>
        /// Processes a request to delete an existing character belonging to the logged-in user.
        /// </summary>
        /// <param name="characterId">The unique ID of the character to be deleted.</param>
        /// <returns>
        /// HTTP response containing the results of the request.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful deletion of character</description></item>
        /// <item><term>404 Not Found</term><description>Response code if character does not exist or does not belong to logged-in user</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpDelete]
		[Route("{characterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500)]
		public IActionResult Delete(int characterId)
		{
			try
			{
				_characterService.AssertUserOwnsCharacter(characterId, UserId, _characterRepository);
				_characterService.DeleteCharacter(characterId, _characterRepository);
				return Ok();
			}
			catch (CharacterNotFoundException)
			{
				_logger.LogWarning($"User {UserId} attempted to delete character {characterId} illegally.");
				return NotFound("A character with that ID does not exist.");
			}
			catch (Exception e)
			{
				_logger.LogError($"Error deleting character {characterId}: {e.Message}", e);
				return StatusCode(500, "An unknown error occurred.");
			}
		}
	}
}
