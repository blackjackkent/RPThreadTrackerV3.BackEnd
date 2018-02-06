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
    }
}
