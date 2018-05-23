namespace RPThreadTrackerV3.Models.ViewModels.PublicViews
{
    using System.Collections.Generic;

    public class PublicThreadDtoCollection : ThreadDtoCollection
    {
	    public PublicViewDto View { get; }

	    public PublicThreadDtoCollection(List<ThreadDto> threads, PublicViewDto view)
	        : base(threads)
	    {
		    View = view;
	    }
    }
}
