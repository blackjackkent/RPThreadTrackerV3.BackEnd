// <copyright file="Thread.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using Interfaces.Data;

    /// <summary>
    /// Data-layer representation of a threaded interaction tracked by a user.
    /// </summary>
    public class Thread : IEntity
    {
        /// <summary>
        /// Gets or sets the unique ID of the thread.
        /// </summary>
        /// <value>
        /// The unique ID of the thread.
        /// </value>
		public int ThreadId { get; set;  }

        /// <summary>
        /// Gets or sets the unique ID of the character associated with this thread.
        /// </summary>
        /// <value>
        /// The unique ID of the character.
        /// </value>
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the character associated with this thread.
        /// </summary>
        /// <value>
        /// The character associated with this thread.
        /// </value>
        public Character Character { get; set; }

        /// <summary>
        /// Gets or sets the ID of the post on its hosting platform.
        /// </summary>
        /// <value>
        /// The post ID on its hosting platform.
        /// </value>
        public string PostId { get; set; }

        /// <summary>
        /// Gets or sets the title of the thread as stored in the tracker (not necessarily related to
        /// its title on the thread's hosting platform).
        /// </summary>
        /// <value>
        /// The title of the thread as stored in the tracker.
        /// </value>
        public string UserTitle { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user's partner on the thread's hosting platform.
        /// (Can be left blank.)
        /// </summary>
        /// <value>
        /// The partner URL identifier, or null if the user did not provide it.
        /// </value>
        public string PartnerUrlIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has archived this thread.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this thread has been archived; otherwise, <c>false</c>.
        /// </value>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the date on which the user last marked this thread as queued.
        /// </summary>
        /// <value>
        /// The date on which the user last marked this thread queued, or <c>null</c>
        /// if they have never done so.
        /// </value>
        public DateTime? DateMarkedQueued { get; set; }

		/// <summary>
		/// Gets or sets a text-block description of the content of the thread.
		/// </summary>
		/// <value>
		/// A summary of the content of the thread.
		/// </value>
		public string Description { get; set; }

        /// <summary>
        /// Gets or sets the thread tags associated with this thread.
        /// </summary>
        /// <value>
        /// The thread tags associated with this thread.
        /// </value>
        public virtual List<ThreadTag> ThreadTags { get; set;  }
    }
}
