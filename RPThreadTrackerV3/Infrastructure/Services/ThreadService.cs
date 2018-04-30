namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using AutoMapper;
	using Enums;
	using Exceptions;
	using Exceptions.Thread;
	using Interfaces.Data;
	using Interfaces.Services;
	using Models.DomainModels;

	public class ThreadService : IThreadService
    {
	    public IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var entities = threadRepository.GetWhere(
			    t => t.Character.UserId == userId
			         && t.IsArchived == isArchived
			         && !t.Character.IsOnHiatus,
			    new List<string> { "Character", "ThreadTags" }
		    ).ToList();
		    return entities.Select(mapper.Map<Thread>).ToList();
	    }

	    public Dictionary<int, List<Thread>> GetThreadsByCharacter(string userId, bool includeArchived, bool includeHiatused, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var entities = threadRepository.GetWhere(
			    t => t.Character.UserId == userId
			         && (includeArchived || !t.IsArchived)
			         && (includeHiatused || !t.Character.IsOnHiatus),
			    new List<string> {"Character"}
		    ).ToList();
		    var models = entities.Select(mapper.Map<Thread>).ToList();
			var groups = models.GroupBy(x => x.CharacterId);
			var dictionary = groups.ToDictionary(x => x.Key, x => x.ToList());
		    return dictionary;
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

	    public void AssertUserOwnsThread(int? threadId, string userId, IRepository<Data.Entities.Thread> threadRepository)
	    {
		    var threadExistsForUser =
			    threadRepository.ExistsWhere(t => t.Character.UserId == userId && t.ThreadId == threadId);
		    if (!threadExistsForUser)
		    {
			    throw new ThreadNotFoundException();
		    }
		}

	    public Thread UpdateThread(Thread thread, string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
			var entity = mapper.Map<Data.Entities.Thread>(thread);
		    var result = threadRepository.Update(thread.ThreadId.ToString(), entity);
			return mapper.Map<Thread>(result);
	    }

	    public void DeleteThread(int threadId, IRepository<Data.Entities.Thread> threadRepository)
	    {
		    var entity = threadRepository.GetWhere(t => t.ThreadId == threadId).FirstOrDefault();
		    if (entity == null)
		    {
			    throw new ThreadNotFoundException();
		    }
		    threadRepository.Delete(entity);
	    }

	    public Thread CreateThread(Thread model, string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var entity = mapper.Map<Data.Entities.Thread>(model);
		    var createdEntity = threadRepository.Create(entity);
		    return mapper.Map<Thread>(createdEntity);
	    }
    }
}
