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
                Description = null,
                IsArchived = false,
                PartnerUrlIdentifier = "my-partner",
                ThreadHomeUrl = "http://www.test.com",
                Character = new CharacterDto(),
                ThreadTags = new List<ThreadTagDto>
                {
                    new ThreadTagDto
                    {
                        ThreadId = 1357,
                        TagText = "My Tag",
                        ThreadTagId = "7531"
                    }
                },
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

            [Fact]
            public void ThrowsErrorWhenDescriptionTooLong()
            {
                // Arrange
                _dto.Description = "7fcZ5JGesziIOYshyElc6jnHPv4Ld6eCajXxzl0pJ3Cahajyvxgyu8i9Iblay5F81vlO8dO7QeSYCfiZ7gdDDfpj1aJ2TBtVNWfZBcYugB1KC2hrIEGd423X7ICWXFYE1E2q7vYHV6KL3AC8w03BKAJTGTIXgLnjG3yPHzmIre1OkgiNeoGrocOQSjTEikpczGY1Qtii7eFMyf5McTG7RalGSG6Fu0yJDi4VLCQ5H6UqWE2sfGSDXi8l5Vv";

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidThreadException>();
            }
        }
    }
}
