namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Collections.Generic;
	using System.Linq;
	using AutoMapper;
	using Entities = Data.Entities;
	using Exceptions;
	using Interfaces.Data;
	using Interfaces.Services;
	using Models.DomainModels;

	public class CharacterService : ICharacterService
    {
	    public void AssertUserOwnsCharacter(int characterId, string userId, IRepository<Entities.Character> characterRepository)
	    {
		    var characterExistsForUser =
			    characterRepository.ExistsWhere(c => c.UserId == userId && c.CharacterId == characterId);
		    if (!characterExistsForUser)
		    {
			    throw new CharacterNotFoundException();
		    }
		}

	    public IEnumerable<Character> GetCharacters(string userId, IRepository<Entities.Character> characterRepository, IMapper mapper)
	    {
		    var entities = characterRepository.GetWhere(c => c.UserId == userId).ToList();
		    return entities.Select(mapper.Map<Character>).ToList();
	    }
    }
}
