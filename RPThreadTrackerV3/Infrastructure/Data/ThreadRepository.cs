namespace RPThreadTrackerV3.Infrastructure.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Entities;
	using Exceptions;

	public class ThreadRepository : BaseRepository<Thread>
	{
		public ThreadRepository(TrackerContext context) : base(context)
		{
		}

		public override Thread Update(string id, Thread entity)
		{
			var existingThread = GetWhere(t => t.ThreadId == entity.ThreadId, new List<string> { "ThreadTags", "Character" }).FirstOrDefault();
			if (existingThread == null)
			{
				throw new ThreadNotFoundException();
			}
			_context.Entry(existingThread).CurrentValues.SetValues(entity);
			foreach (var existingTag in existingThread.ThreadTags.ToList())
			{
				if (entity.ThreadTags.All(t => t.ThreadTagId != existingTag.ThreadTagId))
				{
					_context.ThreadTags.Remove(existingTag);
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
						_context.Entry(existingTag).CurrentValues.SetValues(updatedTag);
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
			_context.SaveChanges();
			_context.Entry(existingThread).Reload();
			if (existingThread.Character == null)
				_context.Entry(existingThread).Reference(p => p.Character).Load();
			return existingThread;
		}
    }
}
