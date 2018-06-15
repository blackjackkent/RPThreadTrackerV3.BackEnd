// <copyright file="ThreadDtoTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using BackEnd.Infrastructure.Exceptions.Thread;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Xunit;

    [Trait("Class", "ThreadDto")]
    public class ThreadDtoTests
    {
        private readonly ThreadDto _dto;

        public ThreadDtoTests()
        {
            _dto = new ThreadDto
            {
                CharacterId = 12345,
                ThreadId = 1357,
                PostId = "98765",
                DateMarkedQueued = DateTime.UtcNow,
                IsArchived = false,
                PartnerUrlIdentifier = "my-partner",
                ThreadHomeUrl = "http://www.test.com",
                Character = new CharacterDto(),
                ThreadTags = new List<ThreadTagDto>(),
                UserTitle = "My Title"
            };
        }

        public class AssertIsValid : ThreadDtoTests
        {
            [Fact]
            public void ThrowsNoErrorWhenValid()
            {
                // Act
                _dto.AssertIsValid();
            }

            [Fact]
            public void ThrowsErrorWhenTitleIsMissing()
            {
                // Arrange
                _dto.UserTitle = string.Empty;

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidThreadException>();
            }

            [Fact]
            public void ThrowsErrorWhenPostIdInvalid()
            {
                // Arrange
                _dto.PostId = "blahblah1234";

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidThreadException>();
            }
        }
    }
}
