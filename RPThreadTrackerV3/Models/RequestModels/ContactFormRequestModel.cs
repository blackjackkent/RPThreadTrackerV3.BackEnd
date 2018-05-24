// <copyright file="ContactFormRequestModel.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.RequestModels
{
    /// <summary>
    /// Request model containing data about a user's rmessage to the site admin.
    /// </summary>
    public class ContactFormRequestModel
    {
        /// <summary>
        /// Gets or sets the message being sent.
        /// </summary>
        /// <value>
        /// The message being sent.
        /// </value>
        public string Message { get; set; }
    }
}
