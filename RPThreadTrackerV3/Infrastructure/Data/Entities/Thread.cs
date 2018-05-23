namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using System;
	using System.Collections.Generic;
	using Interfaces.Data;

    public class Thread : IEntity
    {
		public int ThreadId { get; }
		public int CharacterId { get; }
		public Character Character { get; }
		public string PostId { get; }
		public string UserTitle { get; }
		public string PartnerUrlIdentifier { get; }
		public bool IsArchived { get; }
		public DateTime? DateMarkedQueued { get; }
		public virtual List<ThreadTag> ThreadTags { get; }
    }
}
