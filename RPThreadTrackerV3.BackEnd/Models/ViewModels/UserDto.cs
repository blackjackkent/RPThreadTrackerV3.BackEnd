// <copyright file="UserDto.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.ViewModels
{
    /// <summary>
    /// View model representation of a user.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the unique ID of the user
        /// </summary>
        /// <value>
        /// The unique ID of the user.
        /// </value>
		public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        /// <value>
        /// The user's email.
        /// </value>
        public string Email { get; set; }
	}
}
