// <copyright file="MockChildController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.TestHelpers
{
    using BackEnd.Controllers;

    public class MockChildController : BaseController
    {
        public string RetrieveUserId()
        {
            return UserId;
        }
    }
}
