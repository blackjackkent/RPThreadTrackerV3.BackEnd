namespace RPThreadTrackerV3.TumblrClient.Models.DomainModels
{
	/// <summary>
	/// Class representing a Note object as represented by the Tumblr public API
	/// </summary>
	/// <remarks>
	/// This can represent a 'like', 'reply', or 'reblog', though the tracker
	/// is generally only interested in reblogs.
	/// </remarks>
	public class Note
	{
		/// <summary>
		/// Gets or sets shortname of blog which generated the note
		/// </summary>
		/// <value>
		/// String value of blog shortname (http://{shortname}.tumblr.com)
		/// </value>
		public string BlogName { get; set; }

		/// <summary>
		/// Gets or sets base URL of blog which generated the note
		/// </summary>
		/// <value>
		/// String value of blog base URL
		/// </value>
		public string BlogUrl { get; set; }

		/// <summary>
		/// Gets or sets reblogged post's unique identifier value
		/// </summary>
		/// <value>
		/// Unique numerical identifer of new post if note is a reblog
		/// </value>
		public string PostId { get; set; }

		/// <summary>
		/// Gets or sets note timestamp
		/// </summary>
		/// <value>
		/// Epoch timestamp representing time at which note was generated
		/// </value>
		public long Timestamp { get; set; }

		/// <summary>
		/// Gets or sets value representing note type
		/// </summary>
		/// <value>
		/// 'reblog', 'like', or 'reply'
		/// </value>
		public string Type { get; set; }
	}
}