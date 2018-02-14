namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Models.DomainModels;
	using Models.ViewModels;
	using Resolvers;

	public class CharacterMapper : Profile
	{
		public CharacterMapper()
		{
			CreateMap<Character, Data.Entities.Character>()
				.ForMember(d => d.User, o => o.Ignore())
				.ReverseMap();
			CreateMap<Character, CharacterDto>()
				.ForMember(d => d.HomeUrl, o => o.ResolveUsing<CharacterHomeUrlResolver>())
				.ReverseMap();
		}
	}
}
