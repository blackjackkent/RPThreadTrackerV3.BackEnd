namespace RPThreadTrackerV3.TumblrClient.Models.ResponseModels
{
	using System;

	public class ThreadDataDto
    {
		public string PostId { get; set; }
		public DateTime LastPostDate { get; set; }
		public string LastPosterShortname { get; set; }
		public string LastPostUrl { get; set; }
    }
}
