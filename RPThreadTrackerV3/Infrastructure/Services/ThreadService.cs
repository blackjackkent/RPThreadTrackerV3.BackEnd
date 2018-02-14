namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Collections.Generic;
	using System.Linq;
	using AutoMapper;
	using Enums;
	using Exceptions;
	using Interfaces.Data;
	using Interfaces.Services;
	using Models.DomainModels;

	public class ThreadService : IThreadService
    {
	    public IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper, IRedisClient redisClient)
	    {
		    var key = $"{CacheConstants.THREAD_KEY}{userId}";
		    var result = redisClient.Get<List<Thread>>(key);
		    if (result != null)
		    {
			    return result;
		    }
		    var entities = threadRepository.GetWhere(
			    t => t.Character.UserId == userId
			         && t.IsArchived == isArchived
			         && !t.Character.IsOnHiatus,
			    new List<string> { "Character", "ThreadTags" }
		    ).ToList();
		    result = entities.Select(mapper.Map<Thread>).ToList();
		    redisClient.Set<List<Thread>>(key, result);
		    return result;
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

	    public Thread UpdateThread(Thread thread, string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper, IRedisClient redisClient)
	    {
			var entity = mapper.Map<Data.Entities.Thread>(thread);
		    var result = threadRepository.Update(thread.ThreadId.ToString(), entity);
		    var key = $"{CacheConstants.THREAD_KEY}{userId}";
			redisClient.Delete(key);
			return mapper.Map<Thread>(result);
	    }
    }
}
