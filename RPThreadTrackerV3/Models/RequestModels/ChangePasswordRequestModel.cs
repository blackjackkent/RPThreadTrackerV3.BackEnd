// <copyright file="ChangePasswordRequestModel.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.RequestModels
{
    /// <summary>
    /// Request model containing data about a user's request to change their account password.
    /// </summary>
    public class ChangePasswordRequestModel
    {
        /// <summary>
        /// Gets or sets the user's current password.
        /// </summary>
        /// <value>
        /// The user's current password.
        /// </value>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Gets or sets the user's new password.
        /// </summary>
        /// <value>
        /// The user's new password.
        /// </value>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the user's confirmation of their new password.
        /// </summary>
        /// <value>
        /// The user's confirmation of their new password.
        /// </value>
        public string ConfirmNewPassword { get; set; }
    }
}
