// <copyright file="ThreadService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AutoMapper;
    using Exceptions.Thread;
    using Interfaces.Data;
    using Interfaces.Services;
    using Models.DomainModels;
    using Models.DomainModels.PublicViews;
    using ThreadTag = Data.Entities.ThreadTag;

    /// <inheritdoc />
    public class ThreadService : IThreadService
    {
        /// <inheritdoc />
        public IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
	    {
		    var entities = threadRepository.GetWhere(
			    t => t.Character.UserId == userId
			         && t.IsArchived == isArchived
			         && !t.Character.IsOnHiatus,
			    new List<string> { "Character", "ThreadTags" }).ToList();
		    return entities.Select(mapper.Map<Thread>).ToList();
	    }

        /// <inheritdoc />
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

        /// <exception cref="ThreadNotFoundException">Thrown if a thread with the given ID and user ID could not be found.</exception>
        /// <inheritdoc />
        public void AssertUserOwnsThread(int? threadId, string userId, IRepository<Data.Entities.Thread> threadRepository)
	    {
		    var threadExistsForUser =
			    threadRepository.ExistsWhere(t => t.Character.UserId == userId && t.ThreadId == threadId);
		    if (!threadExistsForUser)
		    {
			    throw new ThreadNotFoundException();
		    }
		}

        /// <inheritdoc />
        public Thread UpdateThread(Thread thread, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
        {
			var entity = mapper.Map<Data.Entities.Thread>(thread);
		    var result = threadRepository.Update(thread.ThreadId.ToString(CultureInfo.CurrentCulture), entity);
			return mapper.Map<Thread>(result);
	    }

        /// <exception cref="ThreadNotFoundException">Thrown when a thread for the given ID could not be found.</exception>
        /// <inheritdoc />
        public void DeleteThread(int threadId, IRepository<Data.Entities.Thread> threadRepository)
	    {
		    var entity = threadRepository.GetWhere(t => t.ThreadId == threadId).FirstOrDefault();
		    if (entity == null)
		    {
			    throw new ThreadNotFoundException();
		    }
		    threadRepository.Delete(entity);
	    }

        /// <inheritdoc />
        public Thread CreateThread(Thread model, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
        {
		    var entity = mapper.Map<Data.Entities.Thread>(model);
		    var createdEntity = threadRepository.Create(entity);
		    return mapper.Map<Thread>(createdEntity);
	    }

        /// <inheritdoc />
        public IEnumerable<string> GetAllTags(string userId, IRepository<Data.Entities.Thread> threadRepository, IMapper mapper)
        {
            var threads = threadRepository.GetWhere(t => t.Character.UserId == userId, new List<string> { "ThreadTags" })
                .ToList();
            var rawTags = threads.SelectMany(t => t.ThreadTags);
            var deduplicated = rawTags.GroupBy(t => t.TagText.ToUpperInvariant()).Select(g => g.First());
            return deduplicated.Select(t => t.TagText);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void ReplaceTag(string currentTag, string replacementTag, string userId, IRepository<ThreadTag> tagRepository, IMapper mapper)
        {
            var normalized = currentTag.ToLower();
            var existingTags = tagRepository.GetWhere(t => t.TagText.ToLower() == normalized && t.Thread.Character.UserId == userId).ToList();
            foreach (var tag in existingTags)
            {
                tag.TagText = replacementTag;
                tagRepository.Update(tag.ThreadTagId, tag);
            }
        }

        /// <inheritdoc />
        public void DeleteTag(string tagText, string userId, IRepository<ThreadTag> tagRepository, IMapper mapper)
        {
            var normalized = tagText.ToLower();
            var existingTags = tagRepository.GetWhere(t => t.TagText.ToLower() == normalized && t.Thread.Character.UserId == userId).ToList();
            foreach (var tag in existingTags)
            {
                tagRepository.Delete(tag);
            }
        }
    }
}
