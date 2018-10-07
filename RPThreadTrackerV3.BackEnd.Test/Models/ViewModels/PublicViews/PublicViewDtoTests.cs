// <copyright file="PublicViewDtoTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels.PublicViews
{
    using System;
    using System.Collections.Generic;
    using BackEnd.Infrastructure.Exceptions.PublicViews;
    using BackEnd.Models.ViewModels.PublicViews;
    using FluentAssertions;
    using Moq;
    using Xunit;

    [Trait("Class", "PublicViewDto")]
    public class PublicViewDtoTests
    {
        private readonly PublicViewDto _dto;

        public PublicViewDtoTests()
        {
            var turnFilter = new Mock<PublicTurnFilterDto>();
            turnFilter.Setup(f => f.AssertIsValid());
            _dto = new PublicViewDto
            {
                CharacterIds = new List<int> { 1, 2 },
                Columns = new List<string> { "column1", "column2" },
                Name = "My Public View",
                Slug = "my-public-view",
                SortDescending = true,
                SortKey = "column1",
                Tags = new List<string>(),
                TurnFilter = turnFilter.Object,
                UserId = "12345"
            };
        }

        public class AssertIsValid : PublicViewDtoTests
        {
            [Fact]
            public void ThrowsNoErrorWhenValid()
            {
                // Act
                _dto.AssertIsValid();
            }

            [Fact]
            public void ThrowsErrorWhenNameMissing()
            {
                // Arrange
                _dto.Name = string.Empty;

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidPublicViewException>();
            }

            [Fact]
            public void ThrowsErrorWhenSlugMissing()
            {
                // Arrange
                _dto.Slug = string.Empty;

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidPublicViewException>();
            }

            [Fact]
            public void ThrowsErrorWhenSlugInvalid()
            {
                // Arrange
                _dto.Slug = "my invalid slug";

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidPublicViewException>();
            }

            [Fact]
            public void ThrowsErrorWhenNoColumns()
            {
                // Arrange
                _dto.Columns = new List<string>();

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidPublicViewException>();
            }

            [Fact]
            public void ThrowsErrorWhenSlugReserved()
            {
                // Arrange
                _dto.Slug = "yourturn";

                // Act
                Action action = () => _dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidPublicViewException>();
            }
        }
    }
}
