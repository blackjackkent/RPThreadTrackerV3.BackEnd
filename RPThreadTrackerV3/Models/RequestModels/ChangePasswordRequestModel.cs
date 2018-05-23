// <copyright file="ChangePasswordRequestModel.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.RequestModels
{
    public class ChangePasswordRequestModel
    {
	    public string CurrentPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmNewPassword { get; set; }
    }
}
