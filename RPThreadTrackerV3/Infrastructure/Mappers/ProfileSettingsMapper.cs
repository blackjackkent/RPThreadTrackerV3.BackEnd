namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Data.Entities;
	using Models.DomainModels;
	using Models.ViewModels;

	public class ProfileSettingsMapper : Profile
	{
		public ProfileSettingsMapper()
		{
			CreateMap<ProfileSettings, ProfileSettingsCollection>()
				.ForMember(d => d.ProfileSettingsCollectionId, o => o.MapFrom(s => s.SettingsId))
				.ReverseMap()
				.ForMember(d => d.SettingsId, o => o.MapFrom(s => s.ProfileSettingsCollectionId));
			CreateMap<ProfileSettings, ProfileSettingsDto>()
				.ReverseMap();
		}
	}
}
