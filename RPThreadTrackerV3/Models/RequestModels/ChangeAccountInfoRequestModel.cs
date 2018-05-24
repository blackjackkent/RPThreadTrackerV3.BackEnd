// <copyright file="ChangeAccountInfoRequestModel.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.RequestModels
{
    /// <summary>
    /// Request model containing data about a user's request to change their account information.
    /// </summary>
    public class ChangeAccountInfoRequestModel
    {
        /// <summary>
        /// Gets or sets the email with which the user's account should be updated.
        /// </summary>
        /// <value>
        /// The email with which the user's account should be updated.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the username with which the user's account should be updated.
        /// </summary>
        /// <value>
        /// The username with which the user's account should be updated.
        /// </value>
        public string Username { get; set; }
    }
}
