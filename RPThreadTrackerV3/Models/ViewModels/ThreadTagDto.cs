// <copyright file="ThreadTagDto.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.ViewModels
{
    /// <summary>
    /// View model representation of a tag string used to categorize a thread.
    /// </summary>
    public class ThreadTagDto
    {
        /// <summary>
        /// Gets or sets the unique ID of this thread tag.
        /// </summary>
        /// <value>
        /// The unique ID of this thread tag.
        /// </value>
	    public string ThreadTagId { get; set; }

        /// <summary>
        /// Gets or sets the tag text.
        /// </summary>
        /// <value>
        /// The tag text.
        /// </value>
        public string TagText { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the thread with which this tag is associated.
        /// </summary>
        /// <value>
        /// The unique ID of the thread with which this tag is associated.
        /// </value>
        public int ThreadId { get; set; }
	}
}
