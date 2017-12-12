namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
