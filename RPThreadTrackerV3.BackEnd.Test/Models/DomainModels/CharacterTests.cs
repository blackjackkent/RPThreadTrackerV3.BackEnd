// <copyright file="CharacterTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.DomainModels
{
    using BackEnd.Models.DomainModels;
    using FluentAssertions;
    using Xunit;

    public class CharacterTests
    {
        public class Constructor : CharacterTests
        {
            [Fact]
            public void ReturnsEmptyPropertiesIfPostIsNull()
            {
                var character = new Character { CharacterId = 12345 };
                character.CharacterId.Should().Be(12345);
            }
        }
    }
}
