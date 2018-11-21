// <copyright file="ThreadDtoCollection.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Collection of threads and the JSON needed to request their current status from the
    /// thread status microservice.
    /// </summary>
    public class ThreadDtoCollection
    {
        /// <summary>
        /// Gets or sets the threads.
        /// </summary>
        /// <value>
        /// The threads.
        /// </value>
        public List<ThreadDto> Threads { get; set; }

        /// <summary>
        /// Gets or sets the JSON needed to request current status of the threads from the
        /// thread status microservice.
        /// </summary>
        /// <value>
        /// The JSON needed to request current status of the threads from the
        /// thread status microservice.
        /// </value>
        public string ThreadStatusRequestJson { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadDtoCollection"/> class.
        /// </summary>
        /// <param name="threads">The threads.</param>
        public ThreadDtoCollection(List<ThreadDto> threads)
	    {
		    Threads = threads;
		    ThreadStatusRequestJson = GetThreadStatusRequestJson(threads);
	    }

	    private string GetThreadStatusRequestJson(List<ThreadDto> threads)
	    {
		    var objects = threads.Where(t => !string.IsNullOrEmpty(t.PostId)).Select(t => new ThreadStatusRequestItem
		    {
				ThreadId = t.ThreadId,
			    PostId = t.PostId,
				PartnerUrlIdentifier = t.PartnerUrlIdentifier,
				CharacterUrlIdentifier = t.Character.UrlIdentifier,
				DateMarkedQueued = t.DateMarkedQueued,
		    });
		    return JsonConvert.SerializeObject(objects);
	    }

        private class ThreadStatusRequestItem
		{
			public int? ThreadId { get; set; }

			public string PostId { get; set; }

            public string CharacterUrlIdentifier { get; set; }

            public string PartnerUrlIdentifier { get; set; }

            public DateTime? DateMarkedQueued { get; set; }
        }
    }
}
