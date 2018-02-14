namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using AutoMapper;
	using Data;
	using Models.DomainModels;
	using Entities = Infrastructure.Data.Entities;
	using Models.ViewModels;

	public interface ICharacterService
    {
	    void AssertUserOwnsCharacter(int characterId, string userId, IRepository<Entities.Character> characterRepository);
	    IEnumerable<Character> GetCharacters(string userId, IRepository<Entities.Character> characterRepository, IMapper mapper, IRedisClient redisClient);
    }
}
