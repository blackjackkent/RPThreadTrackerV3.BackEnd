// <copyright file="ThreadDto.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Infrastructure.Exceptions.Thread;

    /// <summary>
    /// View model representation of a threaded interaction tracked by a user.
    /// </summary>
    public class ThreadDto
    {
        /// <summary>
        /// Gets the unique ID of the thread.
        /// </summary>
        /// <value>
        /// The unique ID of the thread.
        /// </value>
		public int? ThreadId { get; }

        /// <summary>
        /// Gets the unique ID of the character associated with this thread.
        /// </summary>
        /// <value>
        /// The unique ID of the character.
        /// </value>
        public int CharacterId { get; }

        /// <summary>
        /// Gets the character associated with this thread.
        /// </summary>
        /// <value>
        /// The character associated with this thread.
        /// </value>
        public CharacterDto Character { get; }

        /// <summary>
        /// Gets the ID of the post on its hosting platform.
        /// </summary>
        /// <value>
        /// The post ID on its hosting platform.
        /// </value>
        public string PostId { get; }

        /// <summary>
        /// Gets the title of the thread as stored in the tracker (not necessarily related to
        /// its title on the thread's hosting platform).
        /// </summary>
        /// <value>
        /// The title of the thread as stored in the tracker.
        /// </value>
        public string UserTitle { get; }

        /// <summary>
        /// Gets the unique identifier of the user's partner on the thread's hosting platform.
        /// (Can be left blank.)
        /// </summary>
        /// <value>
        /// The partner URL identifier, or null if the user did not provide it.
        /// </value>
        public string PartnerUrlIdentifier { get; }

        /// <summary>
        /// Gets a value indicating whether the user has archived this thread.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this thread has been archived; otherwise, <c>false</c>.
        /// </value>
        public bool IsArchived { get; }

        /// <summary>
        /// Gets the date on which the user last marked this thread as queued.
        /// </summary>
        /// <value>
        /// The date on which the user last marked this thread queued, or <c>null</c>
        /// if they have never done so.
        /// </value>
        public DateTime? DateMarkedQueued { get; }

        /// <summary>
        /// Gets the thread's home URL (based on the hosting platform).
        /// </summary>
        /// <value>
        /// The thread's home URL (based on the hosting platform).
        /// </value>
        public string ThreadHomeUrl { get; }

        /// <summary>
        /// Gets the thread tags associated with this thread.
        /// </summary>
        /// <value>
        /// The thread tags associated with this thread.
        /// </value>
        public List<ThreadTagDto> ThreadTags { get; }

        /// <summary>
        /// Throws an exception if the thread model is not valid.
        /// </summary>
        /// <exception cref="InvalidThreadException">Thrown if the thread model is not valid.</exception>
        public void AssertIsValid()
		{
			if (string.IsNullOrEmpty(UserTitle))
			{
				throw new InvalidThreadException();
			}
			var regex = new Regex(@"^(\d)+$");
			if (!string.IsNullOrEmpty(PostId) && !regex.IsMatch(PostId))
			{
				throw new InvalidThreadException();
			}
		}
	}
}
