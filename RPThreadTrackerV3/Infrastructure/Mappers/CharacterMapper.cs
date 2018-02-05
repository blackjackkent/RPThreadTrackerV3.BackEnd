namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Models.DomainModels;
	using Models.ViewModels;

	public class CharacterMapper : Profile
	{
		public CharacterMapper()
		{
			CreateMap<Character, Data.Entities.Character>()
				.ReverseMap();
			CreateMap<Character, CharacterDto>()
				.ReverseMap();
		}
	}
}
