namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Models.DomainModels;
	using Models.ViewModels;

	public class ThreadMapper : Profile
	{
		public ThreadMapper()
		{
			CreateMap<Thread, Data.Entities.Thread>()
				.ReverseMap();
			CreateMap<Thread, ThreadDto>()
				.ReverseMap();
		}
	}
}
