namespace RPThreadTrackerV3.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AutoMapper;
    using Exceptions.Characters;
    using Interfaces.Data;
    using Interfaces.Services;
    using Models.DomainModels;
    using Entities = Data.Entities;

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

	    public IEnumerable<Character> GetCharacters(string userId, IRepository<Entities.Character> characterRepository, IMapper mapper, bool includeHiatused = true)
	    {
		    var entities = characterRepository.GetWhere(c => c.UserId == userId).ToList();
		    return entities.Select(mapper.Map<Character>).ToList();
	    }

	    public Character CreateCharacter(Character model, string userId, IRepository<Entities.Character> characterRepository, IMapper mapper)
		{
			var entity = mapper.Map<Entities.Character>(model);
			var createdEntity = characterRepository.Create(entity);
			return mapper.Map<Character>(createdEntity);
		}

	    public Character UpdateCharacter(Character model, string userId, IRepository<Entities.Character> characterRepository, IMapper mapper)
		{
			var entity = mapper.Map<Entities.Character>(model);
			var result = characterRepository.Update(model.CharacterId.ToString(CultureInfo.CurrentCulture), entity);
			return mapper.Map<Character>(result);
		}

	    public void DeleteCharacter(int characterId, IRepository<Entities.Character> characterRepository)
		{
			var entity = characterRepository.GetWhere(t => t.CharacterId == characterId).FirstOrDefault();
			if (entity == null)
			{
				throw new CharacterNotFoundException();
			}
			characterRepository.Delete(entity);
		}
    }
}
