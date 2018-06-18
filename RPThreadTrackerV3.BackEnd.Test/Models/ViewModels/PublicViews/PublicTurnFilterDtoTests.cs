// <copyright file="PublicTurnFilterDtoTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels.PublicViews
{
    using System;
    using BackEnd.Infrastructure.Exceptions.PublicViews;
    using BackEnd.Models.ViewModels.PublicViews;
    using FluentAssertions;
    using Xunit;

    [Trait("Class", "PublicTurnFilterDto")]
    public class PublicTurnFilterDtoTests
    {
        public class AssertIsValid : LegacyPublicViewDtoTests
        {
            [Fact]
            public void ThrowsNoErrorWhenValidWithIncludeMyTurn()
            {
                // Arrange
                var dto = new PublicTurnFilterDto
                {
                    IncludeMyTurn = true
                };

                // Act
                dto.AssertIsValid();
            }

            [Fact]
            public void ThrowsNoErrorWhenValidWithIncludeTheirTurn()
            {
                // Arrange
                var dto = new PublicTurnFilterDto
                {
                    IncludeTheirTurn = true
                };

                // Act
                dto.AssertIsValid();
            }

            [Fact]
            public void ThrowsNoErrorWhenValidWithIncludeQueued()
            {
                // Arrange
                var dto = new PublicTurnFilterDto
                {
                    IncludeQueued = true
                };

                // Act
                dto.AssertIsValid();
            }

            [Fact]
            public void ThrowsNoErrorWhenValidWithIncludeArchived()
            {
                // Arrange
                var dto = new PublicTurnFilterDto
                {
                    IncludeArchived = true
                };

                // Act
                dto.AssertIsValid();
            }

            [Fact]
            public static void ThrowsErrorWhenNoValuesProvided()
            {
                // Arrange
                var dto = new PublicTurnFilterDto();

                // Act
                Action action = () => dto.AssertIsValid();

                // Assert
                action.Should().Throw<InvalidPublicViewException>();
            }
        }
    }
}
