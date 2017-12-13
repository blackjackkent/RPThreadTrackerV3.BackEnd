namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Identity;
	using Models.DomainModels;
	using Models.ViewModels;

	public class UserMapper : Profile
	{
		public UserMapper()
		{
			CreateMap<User, IdentityUser>()
				.ReverseMap();
			CreateMap<User, UserDto>()
				.ReverseMap();
		}
	}
}
