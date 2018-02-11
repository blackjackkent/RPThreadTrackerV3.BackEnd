namespace RPThreadTrackerV3.Models.DomainModels
{
	using System;
	using System.Collections.Generic;

	public class Thread
    {
	    public int ThreadId { get; set; }
	    public int CharacterId { get; set; }
	    public Character Character { get; set; }
	    public string PostId { get; set; }
	    public string UserTitle { get; set; }
	    public string PartnerUrlIdentifier { get; set; }
	    public bool IsArchived { get; set; }
	    public DateTime? DateMarkedQueued { get; set; }
		public List<string> ThreadTags { get; set; }
	}
}
