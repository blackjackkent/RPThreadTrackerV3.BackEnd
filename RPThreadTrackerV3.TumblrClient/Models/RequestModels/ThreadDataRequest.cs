using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPThreadTrackerV3.TumblrClient.Models.RequestModels
{
    public class ThreadDataRequest
    {
		public string PostId { get; set; }
		public string BlogShortname { get; set; }
		public string WatchedShortname { get; set; }
		public bool IsArchived { get; set; }
    }
}
