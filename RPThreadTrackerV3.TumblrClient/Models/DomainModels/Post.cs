namespace RPThreadTrackerV3.TumblrClient.Models.DomainModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Infrastructure.Interfaces;

	/// <inheritdoc cref="IPost"/>
	public class Post : IPost
	{
		/// <inheritdoc cref="IPost"/>
		public string BlogName { get; set; }

		/// <inheritdoc cref="IPost"/>
		public long Id { get; set; }

		/// <inheritdoc cref="IPost"/>
		public List<Note> Notes { get; set; }

		/// <inheritdoc cref="IPost"/>
		public string PostUrl { get; set; }

		/// <inheritdoc cref="IPost"/>
		public long Timestamp { get; set; }

		/// <inheritdoc cref="IPost"/>
		public string Title { get; set; }

		/// <inheritdoc cref="IPost"/>
		public Note GetMostRecentRelevantNote(string blogShortname, string watchedShortname)
		{
			Note mostRecentRelevantNote;
			if (Notes == null || Notes.All(n => n.Type != "reblog"))
			{
				return null;
			}
			if (string.IsNullOrEmpty(watchedShortname))
			{
				mostRecentRelevantNote = Notes.OrderByDescending(n => n.Timestamp).FirstOrDefault(n => n.Type == "reblog");
			}
			else
			{
				mostRecentRelevantNote = Notes.OrderByDescending(n => n.Timestamp).FirstOrDefault(n =>
					n.Type == "reblog" && (string.Equals(n.BlogName, watchedShortname, StringComparison.OrdinalIgnoreCase)
					                       || string.Equals(n.BlogName, blogShortname, StringComparison.OrdinalIgnoreCase)));
			}

			return mostRecentRelevantNote;
		}
	}
}