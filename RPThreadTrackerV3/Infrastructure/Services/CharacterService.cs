namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Linq;
	using Data.Entities;
	using Exceptions;
	using Interfaces.Data;
	using Interfaces.Services;

    public class CharacterService : ICharacterService
    {
	    public void AssertUserOwnsCharacter(int characterId, string userId, IRepository<Character> characterRepository)
	    {
		    var characterExistsForUser =
			    characterRepository.ExistsWhere(c => c.UserId == userId && c.CharacterId == characterId);
		    if (!characterExistsForUser)
		    {
			    throw new CharacterNotFoundException();
		    }
		}
    }
}
