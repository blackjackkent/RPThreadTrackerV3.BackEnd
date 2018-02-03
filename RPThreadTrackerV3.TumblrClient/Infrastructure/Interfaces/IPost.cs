namespace RPThreadTrackerV3.TumblrClient.Infrastructure.Interfaces
{
	using System.Collections.Generic;
	using Models.DomainModels;

	/// <summary>
	/// Class representing a Post object as represented by the Tumblr public API
	/// </summary>
	public interface IPost
	{
		/// <summary>
		/// Gets or sets shortname of blog associated with post
		/// </summary>
		/// <value>
		/// String value of blog shortname (http://{shortname}.tumblr.com)
		/// </value>
		string BlogName { get; set; }

		/// <summary>
		/// Gets or sets the post's unique identifier value
		/// </summary>
		/// <value>
		/// Unique numerical identifer of post
		/// </value>
		long Id { get; set; }

		/// <summary>
		/// Gets or sets collection of <see cref="Note"/> objects associated with post
		/// </summary>
		/// <value>
		/// Note information (likes and reblogs and replies) for post
		/// </value>
		List<Note> Notes { get; set; }

		/// <summary>
		/// Gets or sets the URL at which the post can be viewed on Tumblr
		/// </summary>
		/// <value>
		/// The post's URL string
		/// </value>
		string PostUrl { get; set; }

		/// <summary>
		/// Gets or sets post timestamp
		/// </summary>
		/// <value>
		/// Epoch timestamp representing time at which post was created
		/// </value>
		long Timestamp { get; set; }

		/// <summary>
		/// Gets or sets title of post on Tumblr
		/// </summary>
		/// <value>
		/// String title applied to Tumblr post
		/// </value>
		string Title { get; set; }

		/// <summary>
		/// Processes list of notes attached to post and retrieves the most recent reblog
		/// for tracker purposes
		/// </summary>
		/// <param name="blogShortname">Tracker user's blog shortname (for determining if it is user's turn or not)</param>
		/// <param name="watchedShortname">Optional watched shortname (to specifically track this value's posts as relevant)</param>
		/// <returns>Single <see cref="Note"/> object.</returns>
		Note GetMostRecentRelevantNote(string blogShortname, string watchedShortname);
	}
}