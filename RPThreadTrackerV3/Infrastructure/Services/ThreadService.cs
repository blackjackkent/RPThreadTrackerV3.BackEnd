namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Collections.Generic;
	using System.Linq;
	using AutoMapper;
	using Interfaces.Data;
	using Interfaces.Services;
	using Models.DomainModels;

	public class ThreadService : IThreadService
    {
	    public IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var result = threadRepository.GetWhere(
					t => t.Character.UserId == userId && t.IsArchived == isArchived, 
					new List<string> { "Character" }
				).ToList();
		    return result.Select(mapper.Map<Thread>).ToList();
	    }
    }
}
