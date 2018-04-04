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
	    IEnumerable<Character> GetCharacters(string userId, IRepository<Entities.Character> characterRepository, IMapper mapper, bool includeHiatused = true);
	    Character CreateCharacter(Character model, string userId, IRepository<Entities.Character> characterRepository, IMapper mapper);
	    Character UpdateCharacter(Character model, string userId, IRepository<Entities.Character> characterRepository, IMapper mapper);
	    void DeleteCharacter(int characterId, IRepository<Entities.Character> characterRepository);
    }
}
