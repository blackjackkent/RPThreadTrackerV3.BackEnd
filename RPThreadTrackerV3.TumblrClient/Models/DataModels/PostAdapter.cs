namespace RPThreadTrackerV3.TumblrClient.Models.DataModels
{
	using System;
	using System.Linq;
	using DontPanic.TumblrSharp;
	using DontPanic.TumblrSharp.Client;

    public class PostAdapter
    {
	    private readonly BasePost _post;

	    public PostAdapter(BasePost post)
	    {
		    _post = post;
	    }

	    public string Id => _post.Id.ToString();
	    public DateTime Timestamp => _post.Timestamp;
	    public string BlogName => _post.BlogName;
	    public string Url => _post.Url;

	    public BaseNote GetMostRecentRelevantNote(string characterUrlIdentifier, string partnerUrlIdentifier)
	    {
		    BaseNote mostRecentRelevantNote;
		    if (_post.Notes == null || _post.Notes.All(n => n.Type != NoteType.Reblog))
		    {
			    return null;
		    }
		    if (string.IsNullOrEmpty(partnerUrlIdentifier))
		    {
			    mostRecentRelevantNote = _post.Notes.OrderByDescending(n => n.Timestamp).FirstOrDefault(n => n.Type == NoteType.Reblog);
		    }
		    else
		    {
			    mostRecentRelevantNote = _post.Notes.OrderByDescending(n => n.Timestamp).FirstOrDefault(n =>
				    n.Type == NoteType.Reblog && (string.Equals(n.BlogName, partnerUrlIdentifier, StringComparison.OrdinalIgnoreCase)
				                           || string.Equals(n.BlogName, characterUrlIdentifier, StringComparison.OrdinalIgnoreCase)));
		    }
		    return mostRecentRelevantNote;
	    }
	}
}
