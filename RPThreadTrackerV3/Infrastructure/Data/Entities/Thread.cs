namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using System;
	using Interfaces.Data;

    public class Thread : IEntity
    {
		public int ThreadId { get; set; }
		public int CharacterId { get; set; }
		public Character Character { get; set; }
		public string PostId { get; set; }
		public string UserTitle { get; set; }
		public string WatchedShortname { get; set; }
		public bool IsArchived { get; set; }
		public DateTime? DateMarkedQueued { get; set; }
    }
}
