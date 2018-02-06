namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;
	using DontPanic.TumblrSharp;
	using DontPanic.TumblrSharp.Client;
	using DontPanic.TumblrSharp.OAuth;
	using Exceptions;
	using Interfaces;
	using Microsoft.Extensions.Configuration;
	using Models.DataModels;
	using Models.ResponseModels;
	using Polly;
	using Polly.Fallback;
	using Polly.Retry;

	/// <inheritdoc cref="ITumblrClient"/>
	public class TumblrSharpClient : ITumblrClient
	{
		private readonly IConfiguration _config;
		private readonly RetryPolicy _retryPolicy;
		private readonly FallbackPolicy<PostAdapter> _notFoundPolicy;

		/// <summary>
		/// Initializes a new instance of the <see cref="TumblrSharpClient"/> class
		/// </summary>
		/// <param name="config">Unity-injected configuration service</param>
		public TumblrSharpClient(IConfiguration config)
		{
			_config = config;
			_retryPolicy = Policy
				.Handle<TumblrException>(e => e.StatusCode == (HttpStatusCode)429)
				.WaitAndRetryAsync(new[]
				{
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(2),
					TimeSpan.FromSeconds(4)
				});
			_notFoundPolicy = Policy<PostAdapter>
				.Handle<TumblrException>(e => e.StatusCode == HttpStatusCode.NotFound)
				.FallbackAsync((PostAdapter)null);
		}

		/// <inheritdoc cref="ITumblrClient"/>
		public async Task<PostAdapter> GetPost(string postId, string characterUrlIdentifier)
		{
			if (string.IsNullOrWhiteSpace(postId) || string.IsNullOrWhiteSpace(characterUrlIdentifier))
			{
				throw new InvalidPostRequestException();
			}
			var post = await RetrieveApiData(postId, characterUrlIdentifier);
			if (post != null)
			{
				return post;
			}
			RefreshApiCache(postId, characterUrlIdentifier);
			var updatedPost = await RetrieveApiData(postId, characterUrlIdentifier);
			if (updatedPost == null)
			{
				throw new PostNotFoundException();
			}
			return updatedPost;
		}

		public ThreadDataDto ParsePost(PostAdapter post, string characterUrlIdentifier, string partnerUrlIdentifier)
		{
			var note = post.GetMostRecentRelevantNote(characterUrlIdentifier, partnerUrlIdentifier);
			var dto = new ThreadDataDto
			{
				PostId = post.Id,
				LastPostDate = note?.Timestamp ?? post.Timestamp,
				LastPosterUrlIdentifier = note?.BlogName ?? post.BlogName,
				LastPostUrl = note != null ? note.BlogUrl + "post/" + note.PostId : post.Url
			};
			return dto;
		}

		private static void RefreshApiCache(string postId, string characterUrlIdentifier)
		{
			var url = "http://" + characterUrlIdentifier + ".tumblr.com/post/" + postId;
			var webRequest = WebRequest.Create(url);
			try
			{
				webRequest.GetResponse().GetResponseStream();
			}
			catch
			{
				// honestly I don't care if it 404ed
			}
		}

		private async Task<PostAdapter> RetrieveApiData(string postId, string characterUrlIdentifier)
		{
			var factory = new TumblrClientFactory();
			var token = new Token(_config["oauthToken"], _config["oauthSecret"]);
			using (var client = factory.Create<TumblrClient>(_config["consumerKey"], _config["consumerSecret"], token))
			{
				var parameters = new MethodParameterSet {{"notes_info", true}, {"id", postId}};
				return await _notFoundPolicy.WrapAsync(_retryPolicy).ExecuteAsync(async () =>
				{
					var posts = await client.CallApiMethodAsync<Posts>(
						new BlogMethod(characterUrlIdentifier, "posts/text", client.OAuthToken, HttpMethod.Get, parameters),
						CancellationToken.None);
					var result = posts.Result.Select(p => new PostAdapter(p)).ToList();
					return result?.FirstOrDefault();
				});
			}
		}
	}
}