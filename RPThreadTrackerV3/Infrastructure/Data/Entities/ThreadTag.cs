namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using System.ComponentModel.DataAnnotations.Schema;

	public class ThreadTag
    {
		[Column("TagId")]
		public int ThreadTagId { get; set; }
		public string TagText { get; set; }
		public int ThreadId { get; set; }
		public Thread Thread { get; set; }
    }
}
