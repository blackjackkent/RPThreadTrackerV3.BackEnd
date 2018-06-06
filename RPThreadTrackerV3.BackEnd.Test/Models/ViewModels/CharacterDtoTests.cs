// <copyright file="CharacterDtoTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels
{
    using BackEnd.Infrastructure.Enums;
    using BackEnd.Infrastructure.Exceptions.Characters;
    using BackEnd.Models.ViewModels;
    using Xunit;

    public class CharacterDtoTests
    {
        private readonly CharacterDto _dto;

        public CharacterDtoTests()
        {
            _dto = new CharacterDto
            {
                CharacterId = 12345,
                UserId = "13579",
                CharacterName = "My Character",
                HomeUrl = "http://www.test.com",
                IsOnHiatus = false,
                PlatformId = Platform.Tumblr,
                UrlIdentifier = "my-character"
            };
        }

        public class AssertIsValid : CharacterDtoTests
        {
            [Fact]
            public void ThrowsNoErrorWhenValid()
            {
                // Act
                _dto.AssertIsValid();
            }

            [Fact]
            public void ThrowsErrorWhenPlatformIsInvalid()
            {
                // Arrange
                _dto.PlatformId = (Platform)12345;

                // Act
                Assert.Throws<InvalidCharacterException>(() => _dto.AssertIsValid());
            }

            [Fact]
            public void ThrowsErrorWhenUrlIdentifierMissing()
            {
                // Arrange
                _dto.UrlIdentifier = string.Empty;

                // Act
                Assert.Throws<InvalidCharacterException>(() => _dto.AssertIsValid());
            }

            [Fact]
            public void ThrowsErrorWhenUrlIdentifierInvalid()
            {
                // Arrange
                _dto.UrlIdentifier = "My Character";

                // Act
                Assert.Throws<InvalidCharacterException>(() => _dto.AssertIsValid());
            }
        }
    }
}
