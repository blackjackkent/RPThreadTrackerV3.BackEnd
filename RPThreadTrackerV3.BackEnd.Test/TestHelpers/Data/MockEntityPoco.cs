// <copyright file="MockEntityPoco.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.TestHelpers.Data
{
    using Interfaces.Data;

    public class MockEntityPoco : IEntity
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public int Size { get; set; }

        public string MockEntityNavigationPropertyId { get; set; }

        public virtual MockEntityNavigationProperty MockEntityNavigationProperty { get; set; }
    }
}
