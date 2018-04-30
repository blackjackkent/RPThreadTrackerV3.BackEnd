using RPThreadTrackerV3.Infrastructure.Exceptions.Characters;

namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Linq;
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
	public class CharacterController : BaseController
	{
		private readonly ILogger<CharacterController> _logger;
		private readonly IMapper _mapper;
		private readonly ICharacterService _characterService;
		private readonly IRepository<Character> _characterRepository;

		public CharacterController(ILogger<CharacterController> logger, IMapper mapper, 
		ICharacterService characterService, IRepository<Character> characterRepository)
		{
			_logger = logger;
			_mapper = mapper;
			_characterService = characterService;
			_characterRepository = characterRepository;
		}

		[HttpGet]
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

		[HttpPost]
		public IActionResult Post([FromBody] CharacterDto character)
		{
			try
			{
				character.AssertIsValid();
				character.UserId = UserId;
				var model = _mapper.Map<Models.DomainModels.Character>(character);
				var createdCharacter = _characterService.CreateCharacter(model, UserId, _characterRepository, _mapper);
				return Ok(createdCharacter);
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

		[HttpPut]
		[Route("{characterId}")]
		public IActionResult Put(int characterId, [FromBody]CharacterDto character)
		{
			try
			{
				character.AssertIsValid();
				_characterService.AssertUserOwnsCharacter(characterId, UserId, _characterRepository);
				var model = _mapper.Map<Models.DomainModels.Character>(character);
				var updatedCharacter = _characterService.UpdateCharacter(model, UserId, _characterRepository, _mapper);
				return Ok(updatedCharacter);
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
		
		[HttpDelete]
		[Route("{characterId}")]
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
