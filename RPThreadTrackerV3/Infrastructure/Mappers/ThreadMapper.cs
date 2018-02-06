namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using AutoMapper;
	using Models.DomainModels;
	using Models.ViewModels;
	using Resolvers;

	public class ThreadMapper : Profile
	{
		public ThreadMapper()
		{
			CreateMap<Thread, Data.Entities.Thread>()
				.ReverseMap();
			CreateMap<Thread, ThreadDto>()
				.ForMember(d => d.ThreadClientUrl, opt => opt.ResolveUsing<ThreadClientUrlResolver>())
				.ReverseMap();
		}
	}
}
