namespace RPThreadTrackerV3.Models.ViewModels
{
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;

	public class ThreadDtoCollection
    {
	    public ThreadDtoCollection(List<ThreadDto> threads)
	    {
		    Threads = threads;
		    ThreadStatusRequestJson = GetThreadStatusRequestJson(threads);
	    }

	    private string GetThreadStatusRequestJson(List<ThreadDto> threads)
	    {
		    var objects = threads.Select(t => new ThreadStatusRequestItem
		    {
			    PostId = t.PostId, 
				PartnerUrlIdentifer = t.PartnerUrlIdentifier, 
				CharacterUrlIdentifier = t.Character.UrlIdentifier
		    });
		    return JsonConvert.SerializeObject(objects);
	    }

	    public List<ThreadDto> Threads { get; set; }
		public string ThreadStatusRequestJson { get; set; }
    }

	public class ThreadStatusRequestItem
	{
		public string PostId { get; set; }
		public string CharacterUrlIdentifier { get; set; }
		public string PartnerUrlIdentifer { get; set; }
	}
}
