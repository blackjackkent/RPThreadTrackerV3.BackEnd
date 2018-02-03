namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Interfaces
{
	using System.Threading.Tasks;

	public interface ITumblrClient
	{
		Task<IPost> GetPost(string postId, string blogShortname);
	}
}