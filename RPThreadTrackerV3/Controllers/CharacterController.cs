namespace RPThreadTrackerV3.Controllers
{
	using System;
	using System.Linq;
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
	public class CharacterController : BaseController
	{
		private readonly ILogger<CharacterController> _logger;
		private readonly IMapper _mapper;
		private readonly ICharacterService _characterService;
		private readonly IRepository<Character> _characterRepository;
		private readonly IRedisClient _redisClient;

		public CharacterController(ILogger<CharacterController> logger, IMapper mapper, 
		ICharacterService characterService, IRepository<Character> characterRepository, IRedisClient redisClient)
		{
			_logger = logger;
			_mapper = mapper;
			_characterService = characterService;
			_characterRepository = characterRepository;
			_redisClient = redisClient;
		}

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				var characters = _characterService.GetCharacters(UserId, _characterRepository, _mapper, _redisClient);
				var result = characters.Select(_mapper.Map<CharacterDto>).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				return StatusCode(500, "An unknown error occurred.");
			}
		}
	}
}
