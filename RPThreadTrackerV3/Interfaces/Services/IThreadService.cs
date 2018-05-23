namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using AutoMapper;
	using Data;
	using Models.DomainModels;
	using Models.DomainModels.PublicViews;
	using Entities = Infrastructure.Data.Entities;

    public interface IThreadService
    {
	    IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Entities.Thread> threadRepository, IMapper mapper);
	    Thread GetThread(int threadId, string userId, IRepository<Entities.Thread> threadRepository, IMapper mapper);
	    Dictionary<int, List<Thread>> GetThreadsByCharacter(string userId, bool includeArchived, bool includeHiatused, IRepository<Entities.Thread> threadRepository, IMapper mapper);
		void AssertUserOwnsThread(int? threadId, string userId, IRepository<Entities.Thread> threadRepository);
	    Thread UpdateThread(Thread model, string userId, IRepository<Entities.Thread> threadRepository, IMapper mapper);
		void DeleteThread(int threadId, IRepository<Entities.Thread> threadRepository);
	    Thread CreateThread(Thread model, string userId, IRepository<Entities.Thread> threadRepository, IMapper mapper);
        IEnumerable<string> GetAllTags(string userId, IRepository<Entities.Thread> threadRepository, IMapper mapper);
	    IEnumerable<Thread> GetThreadsForView(PublicView view, IRepository<Entities.Thread> threadRepository, IMapper mapper);
    }
}
