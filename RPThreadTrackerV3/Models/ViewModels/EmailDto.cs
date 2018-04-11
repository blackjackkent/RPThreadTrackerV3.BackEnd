namespace RPThreadTrackerV3.Models.ViewModels
{
    public class EmailDto
    {
		public string RecipientEmail { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	    public string PlainTextBody { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}
