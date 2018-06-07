// <copyright file="PublicThreadDtoCollectionTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels.PublicViews
{
    using System.Collections.Generic;
    using BackEnd.Models.ViewModels;
    using BackEnd.Models.ViewModels.PublicViews;
    using FluentAssertions;
    using Xunit;

    [Trait("Class", "PublicThreadDtoCollection")]
    public class PublicThreadDtoCollectionTests
    {
        public class Constructor : PublicThreadDtoCollectionTests
        {
            [Fact]
            public void PopulatesViewProperty()
            {
                // Arrange
                var threads = new List<ThreadDto>();
                var view = new PublicViewDto
                {
                    Id = "12345"
                };

                // Act
                var collection = new PublicThreadDtoCollection(threads, view);

                // Assert
                collection.View.Id.Should().Be("12345");
            }
        }
    }
}
