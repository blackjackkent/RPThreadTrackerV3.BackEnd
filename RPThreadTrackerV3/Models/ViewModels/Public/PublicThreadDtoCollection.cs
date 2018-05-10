using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPThreadTrackerV3.Models.ViewModels.Public
{
    public class PublicThreadDtoCollection : ThreadDtoCollection
    {
	    public PublicViewDto View { get; }

	    public PublicThreadDtoCollection(List<ThreadDto> threads, PublicViewDto view) : base(threads)
	    {
		    View = view;
	    }
    }
}
