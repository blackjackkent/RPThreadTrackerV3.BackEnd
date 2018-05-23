namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using AutoMapper;
	using Enums;
	using Exceptions;
	using Exceptions.Thread;
	using Interfaces.Data;
	using Interfaces.Services;
	using Models.DomainModels;
	using Models.DomainModels.PublicViews;

    public class ThreadService : IThreadService
    {
	    public IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var entities = threadRepository.GetWhere(
			    t => t.Character.UserId == userId
			         && t.IsArchived == isArchived
			         && !t.Character.IsOnHiatus,
			    new List<string> { "Character", "ThreadTags" }).ToList();
		    return entities.Select(mapper.Map<Thread>).ToList();
	    }

	    public Dictionary<int, List<Thread>> GetThreadsByCharacter(string userId, bool includeArchived, bool includeHiatused, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var entities = threadRepository.GetWhere(
			    t => t.Character.UserId == userId
			         && (includeArchived || !t.IsArchived)
			         && (includeHiatused || !t.Character.IsOnHiatus),
			    new List<string> { "Character" }).ToList();
		    var models = entities.Select(mapper.Map<Thread>).ToList();
			var groups = models.GroupBy(x => x.CharacterId);
			var dictionary = groups.ToDictionary(x => x.Key, x => x.ToList());
		    return dictionary;
	    }

	    public Thread GetThread(int threadId, string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var result = threadRepository.GetWhere(t => t.Character.UserId == userId && t.ThreadId == threadId, new List<string> { "Character", "ThreadTags" })
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
		    var result = threadRepository.Update(thread.ThreadId.ToString(CultureInfo.CurrentCulture), entity);
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

        public IEnumerable<string> GetAllTags(string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
        {
            var threads = threadRepository.GetWhere(t => t.Character.UserId == userId, new List<string> { "ThreadTags" })
                .ToList();
            var rawTags = threads.SelectMany(t => t.ThreadTags);
            var deduplicated = rawTags.GroupBy(t => t.TagText).Select(g => g.First());
            return deduplicated.Select(t => t.TagText);
        }

	    public IEnumerable<Thread> GetThreadsForView(PublicView view, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var threads = threadRepository.GetWhere(
		            t => t.Character.UserId == view.UserId,
			        new List<string> { "Character", "ThreadTags" })
		        .ToList();
		    var filteredThreads = new List<Data.Entities.Thread>();
		    if (view.TurnFilter.IncludeArchived)
		    {
			    var archivedThreads = threads.Where(t => t.IsArchived);
				filteredThreads.AddRange(archivedThreads);
		    }
		    if (view.TurnFilter.IncludeMyTurn || view.TurnFilter.IncludeTheirTurn || view.TurnFilter.IncludeQueued)
		    {
			    var nonArchivedThreads = threads.Where(t => !t.IsArchived);
				filteredThreads.AddRange(nonArchivedThreads);
		    }
		    if (view.CharacterIds.Any())
		    {
			    filteredThreads = filteredThreads.Where(t => view.CharacterIds.Contains(t.CharacterId)).ToList();
		    }
		    if (view.Tags.Any())
		    {
			    filteredThreads = filteredThreads.Where(t => t.ThreadTags.Select(tt => tt.TagText).Intersect(view.Tags).Any())
				    .ToList();
		    }
		    return mapper.Map<List<Thread>>(filteredThreads);
	    }
    }
}
