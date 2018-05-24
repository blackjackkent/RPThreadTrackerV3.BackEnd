// <copyright file="PublicThreadDtoCollection.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.ViewModels.PublicViews
{
    using System.Collections.Generic;

    /// <summary>
    /// Collection of threads for a particular public view and the JSON needed to request their
    /// current status from the thread status microservice, as well as the associated public view
    /// object.
    /// </summary>
    /// <seealso cref="ThreadDtoCollection" />
    public class PublicThreadDtoCollection : ThreadDtoCollection
    {
        /// <summary>
        /// Gets the public view.
        /// </summary>
        /// <value>
        /// The public view.
        /// </value>
        public PublicViewDto View { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicThreadDtoCollection"/> class.
        /// </summary>
        /// <param name="threads">The threads.</param>
        /// <param name="view">The view.</param>
        public PublicThreadDtoCollection(List<ThreadDto> threads, PublicViewDto view)
	        : base(threads)
	    {
		    View = view;
	    }
    }
}
