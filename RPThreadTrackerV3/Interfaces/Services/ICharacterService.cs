namespace RPThreadTrackerV3.Interfaces.Services
{
	using Data;
	using Infrastructure.Data.Entities;

	public interface ICharacterService
    {
	    void AssertUserOwnsCharacter(int characterId, string userId, IRepository<Character> characterRepository);
    }
}
