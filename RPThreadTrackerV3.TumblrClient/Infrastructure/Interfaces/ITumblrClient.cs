namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Interfaces
{
	using System.Threading.Tasks;
	using Models.DataModels;
	using Models.ResponseModels;

	public interface ITumblrClient
	{
		Task<PostAdapter> GetPost(string postId, string characterUrlIdentifier);
		ThreadDataDto ParsePost(PostAdapter post, string characterUrlIdentifier, string partnerUrlIdentifier);
	}
}