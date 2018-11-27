// <copyright file="ThreadRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Entities;
    using Exceptions.Thread;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class ThreadRepository : BaseRepository<Thread>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ThreadRepository(TrackerContext context)
		    : base(context)
		{
		}

        /// <exception cref="ThreadNotFoundException">Thrown if a thread with the given ID could not be found.</exception>
        /// <inheritdoc />
        public override Thread Update(string id, Thread entity)
		{
			var existingThread = GetWhere(t => t.ThreadId == entity.ThreadId, new List<string> { "ThreadTags", "Character" }).FirstOrDefault();
			if (existingThread == null)
			{
				throw new ThreadNotFoundException();
			}
			Context.Entry(existingThread).CurrentValues.SetValues(entity);
			foreach (var existingTag in existingThread.ThreadTags.ToList())
			{
				if (entity.ThreadTags.All(t => t.ThreadTagId != existingTag.ThreadTagId))
				{
					Context.ThreadTags.Remove(existingTag);
				}
			}
			foreach (var updatedTag in entity.ThreadTags)
			{
				if (string.IsNullOrEmpty(updatedTag.ThreadTagId))
				{
					var newTag = new ThreadTag
					{
						ThreadTagId = Guid.NewGuid().ToString(),
						TagText = updatedTag.TagText,
						ThreadId = existingThread.ThreadId
					};
					existingThread.ThreadTags.Add(newTag);
				}
				else
				{
					var existingTag = existingThread.ThreadTags
						.FirstOrDefault(c => c.ThreadTagId == updatedTag.ThreadTagId);
					if (existingTag != null)
					{
						Context.Entry(existingTag).CurrentValues.SetValues(updatedTag);
					}
					else
					{
						var newTag = new ThreadTag
						{
							ThreadTagId = Guid.NewGuid().ToString(),
							TagText = updatedTag.TagText,
							ThreadId = existingThread.ThreadId
						};
						existingThread.ThreadTags.Add(newTag);
					}
				}
			}
			Context.SaveChanges();
			Context.Entry(existingThread).Reload();
		    if (existingThread.Character == null)
		    {
		        Context.Entry(existingThread).Reference(p => p.Character).Load();
		    }
		    return existingThread;
		}
    }
}
