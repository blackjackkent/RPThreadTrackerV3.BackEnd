// <copyright file="ThreadDtoCollectionTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Newtonsoft.Json;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "ThreadDtoCollection")]
    public class ThreadDtoCollectionTests
    {
        public class Constructor : ThreadDtoCollectionTests
        {
            [Fact]
            public static void PopulatesThreadsFromProvidedList()
            {
                // Arrange
                var threads = new List<ThreadDto>
                {
                    new ThreadDto(),
                    new ThreadDto(),
                    new ThreadDto()
                };

                // Act
                var collection = new ThreadDtoCollection(threads);

                // Assert
                collection.Threads.Count.Should().Be(3);
            }

            [Fact]
            public static void PopulatesThreadJsonFromProvidedList()
            {
                // Arrange
                var threads = new List<ThreadDto>
                {
                    new ThreadDto { ThreadId = 5, PostId = "1", Character = new CharacterDto { CharacterId = 2 } },
                    new ThreadDto { ThreadId = 4, PostId = "2",  Character = new CharacterDto { CharacterId = 3 } },
                    new ThreadDto { ThreadId = 6, PostId = "3",  Character = new CharacterDto { CharacterId = 4 } }
                };

                // Act
                var collection = new ThreadDtoCollection(threads);
                var json = collection.ThreadStatusRequestJson;
                var data = JsonConvert.DeserializeObject<List<MockThreadStatusRequestItem>>(json);

                // Assert
                data.Count.Should().Be(3);
                data.Count(t => t.PostId == "1" && t.ThreadId == 5).Should().Be(1);
                data.Count(t => t.PostId == "2" && t.ThreadId == 4).Should().Be(1);
                data.Count(t => t.PostId == "3" && t.ThreadId == 6).Should().Be(1);
            }
        }
    }
}
