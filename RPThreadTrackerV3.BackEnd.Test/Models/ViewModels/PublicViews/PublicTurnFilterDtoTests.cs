// <copyright file="PublicTurnFilterDtoTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels.PublicViews
{
    using BackEnd.Infrastructure.Exceptions.PublicViews;
    using BackEnd.Models.ViewModels.PublicViews;
    using Xunit;

    public class PublicTurnFilterDtoTests
    {
        public class AssertIsValid : LegacyPublicViewDtoTests
        {
            [Fact]
            public void ThrowsNoErrorWhenValid()
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
            public static void ThrowsErrorWhenNoValuesProvided()
            {
                // Arrange
                var dto = new PublicTurnFilterDto();

                // Act
                Assert.Throws<InvalidPublicViewException>(() => dto.AssertIsValid());
            }
        }
    }
}
