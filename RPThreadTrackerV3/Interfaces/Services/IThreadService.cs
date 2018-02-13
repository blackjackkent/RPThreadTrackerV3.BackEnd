namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using AutoMapper;
	using Data;
	using Entities = Infrastructure.Data.Entities;
	using Models.DomainModels;

	public interface IThreadService
    {
	    IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Entities.Thread> threadRepository, IMapper mapper);
	    Thread GetThread(int threadId, string userId, IRepository<Entities.Thread> threadRepository, IMapper mapper);
		void AssertUserOwnsThread(int threadThreadId, string userId, IRepository<Entities.Thread> threadRepository);
	    Thread UpdateThread(Thread model, IRepository<Entities.Thread> threadRepository, IMapper mapper);
    }
}
