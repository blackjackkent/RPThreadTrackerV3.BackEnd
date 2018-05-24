// <copyright file="ForgotPasswordRequestModel.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.RequestModels
{
    /// <summary>
    /// Request model containing data about a user's request to receive a password reset link via email.
    /// </summary>
    public class ForgotPasswordRequestModel
    {
        /// <summary>
        /// Gets or sets the email where the reset link should be sent.
        /// </summary>
        /// <value>
        /// The email where the reset link should be sent.
        /// </value>
        public string Email { get; set; }
    }
}
