namespace RPThreadTrackerV3.Models.ViewModels
{
	using System;

	public class ThreadDto
	{
		public int ThreadId { get; set; }
		public int CharacterId { get; set; }
		public CharacterDto Character { get; set; }
		public string PostId { get; set; }
		public string UserTitle { get; set; }
		public string WatchedShortname { get; set; }
		public bool IsArchived { get; set; }
		public DateTime? DateMarkedQueued { get; set; }
	}
}
