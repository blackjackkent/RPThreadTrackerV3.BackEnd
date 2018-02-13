namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Collections.Generic;
	using System.Linq;
	using AutoMapper;
	using Exceptions;
	using Interfaces.Data;
	using Interfaces.Services;
	using Models.DomainModels;
	using Models.ViewModels;

	public class ThreadService : IThreadService
    {
	    public IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var result = threadRepository.GetWhere(
					t => t.Character.UserId == userId && t.IsArchived == isArchived, 
					new List<string> { "Character", "ThreadTags" }
				).ToList();
		    return result.Select(mapper.Map<Thread>).ToList();
	    }

	    public Thread GetThread(int threadId, string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var result = threadRepository.GetWhere(t => t.Character.UserId == userId && t.ThreadId == threadId)
			    .FirstOrDefault();
		    if (result == null)
		    {
			    throw new ThreadNotFoundException();
		    }
		    return mapper.Map<Thread>(result);
	    }

	    public void AssertUserOwnsThread(int threadId, string userId, IRepository<Data.Entities.Thread> threadRepository)
	    {
		    var threadExistsForUser =
			    threadRepository.ExistsWhere(t => t.Character.UserId == userId && t.ThreadId == threadId);
		    if (!threadExistsForUser)
		    {
			    throw new ThreadNotFoundException();
		    }
		}

	    public Thread UpdateThread(Thread thread, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
			var entity = mapper.Map<Data.Entities.Thread>(thread);
		    var result = threadRepository.Update(thread.ThreadId.ToString(), entity);
			return mapper.Map<Thread>(result);
	    }
    }
}
