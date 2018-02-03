namespace RPThreadTracker.Infrastructure.Services
{
	using System;
	using System.Linq;
	using DontPanic.TumblrSharp;
	using DontPanic.TumblrSharp.Client;
	using Interfaces;
	using Models.ServiceModels;
	using RPThreadTrackerV3.TumblrClient.Infrastructure.Interfaces;
	using RPThreadTrackerV3.TumblrClient.Models.DomainModels;

	/// <summary>
	/// Adapter class to translate between TumblrSharp post types
	/// and tracker service models
	/// </summary>
	public class TumblrSharpPostAdapter
	{
		/// <summary>
		/// Translates a TumblrSharp post to a tracker service model
		/// based on post type
		/// </summary>
		/// <param name="tumblrSharpPost">The post to be adapted to tracker models</param>
		/// <returns><see cref="IPost"/>Translated post object</returns>
		public IPost GetPost(BasePost tumblrSharpPost)
		{
			if (tumblrSharpPost is TextPost)
			{
				return GetFromTextPost(tumblrSharpPost);
			}
			if (tumblrSharpPost is AnswerPost)
			{
				return GetFromAnswerPost(tumblrSharpPost);
			}
			if (tumblrSharpPost is AudioPost)
			{
				return GetFromAudioPost(tumblrSharpPost);
			}
			if (tumblrSharpPost is ChatPost)
			{
				return GetFromChatPost(tumblrSharpPost);
			}
			if (tumblrSharpPost is LinkPost)
			{
				return GetFromLinkPost(tumblrSharpPost);
			}
			if (tumblrSharpPost is PhotoPost)
			{
				return GetFromPhotoPost(tumblrSharpPost);
			}
			if (tumblrSharpPost is QuotePost)
			{
				return GetFromQuotePost(tumblrSharpPost);
			}
			if (tumblrSharpPost is VideoPost)
			{
				return GetFromVideoPost(tumblrSharpPost);
			}
			return GetFromBasePost(tumblrSharpPost);
		}

		private static IPost GetFromBasePost(BasePost tumblrSharpPost)
		{
			if (tumblrSharpPost == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = tumblrSharpPost?.BlogName,
				Id = tumblrSharpPost?.Id ?? 0,
				PostUrl = tumblrSharpPost?.Url,
				Timestamp = UnixTimestampFromDateTime(tumblrSharpPost?.Timestamp).GetValueOrDefault(),
				Title = null,
				Notes = tumblrSharpPost?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromVideoPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as VideoPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = null,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromQuotePost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as QuotePost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = null,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromPhotoPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as PhotoPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = null,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromLinkPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as LinkPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = post.Title,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromChatPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as ChatPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = post.Title,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromAudioPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as AudioPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = post.TrackName,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromAnswerPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as AnswerPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = null,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static IPost GetFromTextPost(BasePost tumblrSharpPost)
		{
			var post = tumblrSharpPost as TextPost;
			if (post == null)
			{
				return null;
			}
			return new Post
			{
				BlogName = post?.BlogName,
				Id = post?.Id ?? 0,
				PostUrl = post?.Url,
				Timestamp = UnixTimestampFromDateTime(post?.Timestamp).GetValueOrDefault(),
				Title = post?.Title,
				Notes = post?.Notes?.Select(n => new Note
				{
					Timestamp = UnixTimestampFromDateTime(n.Timestamp).GetValueOrDefault(),
					BlogName = n.BlogName,
					BlogUrl = n.BlogUrl,
					PostId = n.PostId,
					Type = ParseNoteType(n.Type)
				}).ToList()
			};
		}

		private static long? UnixTimestampFromDateTime(DateTime? date)
		{
			var unixTimestamp = date?.Ticks - new DateTime(1970, 1, 1).Ticks;
			unixTimestamp /= TimeSpan.TicksPerSecond;
			return unixTimestamp;
		}

		private static string ParseNoteType(NoteType type)
		{
			switch (type)
			{
				case NoteType.Like:
					return "like";
				case NoteType.Reblog:
					return "reblog";
				case NoteType.Reply:
					return "reply";
				case NoteType.Posted:
					return "posted";
				default:
					return string.Empty;
			}
		}
	}
}