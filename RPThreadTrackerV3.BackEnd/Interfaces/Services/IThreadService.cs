// <copyright file="IThreadService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using System.Collections.Generic;
    using AutoMapper;
    using Data;
    using Models.DomainModels;
    using Models.DomainModels.PublicViews;
    using Entities = Infrastructure.Data.Entities;

    /// <summary>
    /// Service for data manipulation relating to threads.
    /// </summary>
    public interface IThreadService
    {
        /// <summary>
        /// Gets all threads associated with a given user.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="isArchived">if set to <c>true</c>, includes threads marked as archived.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>
        /// List of <see cref="Thread" /> objects belonging to the given user.
        /// </returns>
	    IEnumerable<Thread> GetThreads(string userId, bool isArchived, IRepository<Entities.Thread> threadRepository, IMapper mapper);

        /// <summary>
        /// Gets all threads belonging to the given user, sorted by character ID.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="includeArchived">if set to <c>true</c>, includes threads marked as archived.</param>
        /// <param name="includeHiatused">if set to <c>true</c>, includes threads belonging to characters marked as on hiatus.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>A dictionary of <see cref="Thread"/> objects sorted by character ID.</returns>
        Dictionary<int, List<Thread>> GetThreadsByCharacter(string userId, bool includeArchived, bool includeHiatused, IRepository<Entities.Thread> threadRepository, IMapper mapper);

        /// <summary>
        /// Throws an exception if the given user does not own the given thread.
        /// </summary>
        /// <param name="threadId">The unique ID of the thread.</param>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="threadRepository">The thread repository.</param>
        void AssertUserOwnsThread(int? threadId, string userId, IRepository<Entities.Thread> threadRepository);

        /// <summary>
        /// Updates the passed thread.
        /// </summary>
        /// <param name="model">The model containing thread information.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>The updated thread object</returns>
	    Thread UpdateThread(Thread model, IRepository<Entities.Thread> threadRepository, IMapper mapper);

        /// <summary>
        /// Deletes the thread with the passed ID.
        /// </summary>
        /// <param name="threadId">The unique ID of the thread to delete.</param>
        /// <param name="threadRepository">The thread repository.</param>
		void DeleteThread(int threadId, IRepository<Entities.Thread> threadRepository);

        /// <summary>
        /// Creates the passed thread.
        /// </summary>
        /// <param name="model">The model containing thread information.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>The newly created thread object.</returns>
	    Thread CreateThread(Thread model, IRepository<Entities.Thread> threadRepository, IMapper mapper);

        /// <summary>
        /// Gets all tags for all threads belonging to the given user, as strings with duplicates removed.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>A deduplicated list of all the tags for all threads belonging to the given user, as strings.</returns>
        IEnumerable<string> GetAllTags(string userId, IRepository<Entities.Thread> threadRepository, IMapper mapper);

        /// <summary>
        /// Gets all the threads associated with the given public view.
        /// </summary>
        /// <param name="view">The public view for which to retrieve threads.</param>
        /// <param name="threadRepository">The thread repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>
        /// List of <see cref="Thread" /> objects associated with the given public view.
        /// </returns>
        IEnumerable<Thread> GetThreadsForView(PublicView view, IRepository<Entities.Thread> threadRepository, IMapper mapper);
    }
}
