namespace RPThreadTrackerV3.Infrastructure.Mappers
{
	using System.Linq;
	using AutoMapper;
	using Data.Entities;
	using Models.ViewModels;
	using Thread = Models.DomainModels.Thread;

	public class ThreadMapper : Profile
	{
		public ThreadMapper()
		{
			CreateMap<Thread, Data.Entities.Thread>()
				.ReverseMap()
				.ForMember(d => d.ThreadTags, o => o.ResolveUsing(s => s.ThreadTags.Select(t => t.TagText)));
			CreateMap<Thread, ThreadDto>()
				.ReverseMap();
		}
	}
}
