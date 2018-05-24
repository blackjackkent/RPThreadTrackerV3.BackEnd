// <copyright file="ResetPasswordRequestModel.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.RequestModels
{
    /// <summary>
    /// Request model containing data about a user's request to reset their password.
    /// </summary>
    public class ResetPasswordRequestModel
    {
        /// <summary>
        /// Gets or sets the email of the user requesting the reset.
        /// </summary>
        /// <value>
        /// The email of the user requesting the reset.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the code confirming the user can reset their password.
        /// </summary>
        /// <value>
        /// The code confirming the user can reset their password.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        /// <value>
        /// The new password.
        /// </value>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirmed new password.
        /// </summary>
        /// <value>
        /// The confirmed new password.
        /// </value>
        public string ConfirmNewPassword { get; set; }
    }
}
