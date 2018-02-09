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
				.ReverseMap();
			CreateMap<ProfileSettings, ProfileSettingsDto>()
				.ReverseMap();
		}
	}
}
