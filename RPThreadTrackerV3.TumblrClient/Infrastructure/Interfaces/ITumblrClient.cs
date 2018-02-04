namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Interfaces
{
	using System.Threading.Tasks;
	using DontPanic.TumblrSharp.Client;
	using Models.DataModels;
	using Models.ResponseModels;

	public interface ITumblrClient
	{
		Task<PostAdapter> GetPost(string postId, string blogShortname);
		ThreadDataDto ParsePost(PostAdapter post, string blogShortname, string watchedShortname);
	}
}