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
	    public IEnumerable<Thread> GetThreads(string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var result = threadRepository.GetWhere(t => t.Character.UserId == userId).ToList();
		    return result.Select(mapper.Map<Thread>).ToList();
	    }
    }
}
