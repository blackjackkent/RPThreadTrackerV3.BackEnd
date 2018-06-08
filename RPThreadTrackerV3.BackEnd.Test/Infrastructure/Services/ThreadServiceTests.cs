// <copyright file="ThreadServiceTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using AutoMapper;
    using BackEnd.Infrastructure.Data.Entities;
    using BackEnd.Infrastructure.Services;
    using FluentAssertions;
    using Interfaces.Data;
    using Moq;
    using Xunit;
    using DomainModels = BackEnd.Models.DomainModels;

    [Trait("Class", "ThreadService")]
    public class ThreadServiceTests
    {
        private readonly ThreadService _threadService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<Thread>> _mockThreadRepository;

        public ThreadServiceTests()
        {
            _mockThreadRepository = new Mock<IRepository<Thread>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<DomainModels.Thread>(It.IsAny<Thread>()))
                .Returns((Thread entity) => new DomainModels.Thread
                {
                    ThreadId = entity.ThreadId,
                    CharacterId = entity.CharacterId,
                    UserTitle = entity.UserTitle,
                    IsArchived = entity.IsArchived
                });
            _mockMapper.Setup(m => m.Map<Thread>(It.IsAny<DomainModels.Thread>()))
                .Returns((DomainModels.Thread model) => new Thread
                {
                    ThreadId = model.ThreadId,
                    CharacterId = model.CharacterId,
                    UserTitle = model.UserTitle,
                    IsArchived = model.IsArchived
                });

            var threadList = BuildThreadList();
            _mockThreadRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Thread, bool>>>(y => threadList.Any(y.Compile())), It.IsAny<List<string>>())).Returns((Expression<Func<Thread, bool>> predicate, List<string> navigationProperties) => threadList.Where(predicate.Compile()));
            _threadService = new ThreadService();
        }

        private List<Thread> BuildThreadList()
        {
            var character1 = new Character()
            {
                CharacterId = 1,
                CharacterName = "Character 1",
                IsOnHiatus = false,
                UserId = "98765"
            };
            var character2 = new Character()
            {
                CharacterId = 2,
                CharacterName = "Character 2",
                IsOnHiatus = false,
                UserId = "98765"
            };
            var hiatusedCharacter = new Character()
            {
                CharacterId = 3,
                CharacterName = "Hiatused Character",
                IsOnHiatus = true,
                UserId = "98765"
            };
            var anotherUsersCharacter = new Character()
            {
                CharacterId = 4,
                CharacterName = "Another User's Character",
                IsOnHiatus = false,
                UserId = "12345"
            };

            var activeThreadActiveCharacter1 = new Thread
            {
                ThreadId = 1,
                CharacterId = 1,
                Character = character1,
                IsArchived = false,
            };
            var activeThreadActiveCharacter1_2 = new Thread
            {
                ThreadId = 2,
                CharacterId = 1,
                Character = character1,
                IsArchived = false
            };
            var archivedThreadActiveCharacter1 = new Thread
            {
                ThreadId = 3,
                CharacterId = 1,
                Character = character1,
                IsArchived = true
            };
            var activeThreadActiveCharacter2 = new Thread
            {
                ThreadId = 4,
                CharacterId = 2,
                Character = character2,
                IsArchived = false,
            };
            var activeThreadActiveCharacter2_2 = new Thread
            {
                ThreadId = 5,
                CharacterId = 2,
                Character = character2,
                IsArchived = false
            };
            var archivedThreadActiveCharacter2 = new Thread
            {
                ThreadId = 6,
                CharacterId = 2,
                Character = character2,
                IsArchived = true
            };
            var activeThreadHiatusedCharacter = new Thread
            {
                ThreadId = 7,
                CharacterId = 3,
                Character = hiatusedCharacter,
                IsArchived = false,
            };
            var activeThreadHiatusedCharacter2 = new Thread
            {
                ThreadId = 8,
                CharacterId = 3,
                Character = hiatusedCharacter,
                IsArchived = false
            };
            var archivedThreadHiatusedCharacter = new Thread
            {
                ThreadId = 9,
                CharacterId = 3,
                Character = hiatusedCharacter,
                IsArchived = true
            };
            var activeThreadAnotherUsersCharacter = new Thread
            {
                ThreadId = 10,
                CharacterId = 4,
                Character = anotherUsersCharacter,
                IsArchived = false
            };
            var archivedThreadAnotherUsersCharacter = new Thread
            {
                ThreadId = 11,
                CharacterId = 4,
                Character = anotherUsersCharacter,
                IsArchived = true
            };

            var threadList = new List<Thread>
            {
                activeThreadActiveCharacter1,
                activeThreadActiveCharacter1_2,
                archivedThreadActiveCharacter1,
                activeThreadActiveCharacter2,
                activeThreadActiveCharacter2_2,
                archivedThreadActiveCharacter2,
                activeThreadHiatusedCharacter,
                activeThreadHiatusedCharacter2,
                archivedThreadHiatusedCharacter,
                activeThreadAnotherUsersCharacter,
                archivedThreadAnotherUsersCharacter
            };
            return threadList;
        }

        public class GetThreads : ThreadServiceTests
        {
            [Fact]
            public void ReturnsActiveThreadsNotOnHiatusWhenIsArchivedIsFalse()
            {
                // Act
                var result = _threadService.GetThreads("98765", false, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(4);
            }

            [Fact]
            public void ReturnsArchivedThreadsNotOnHiatusWhenIsArchivedIsTrue()
            {
                // Act
                var result = _threadService.GetThreads("98765", true, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(2);
            }
        }

        public class GetThreadsByCharacter : ThreadServiceTests
        {
            [Fact]
            public void ReturnsNonArchivedAndNonHiatusedThreadsWhenIncludeArchivedIsFalseAndIncludeHiatusedIsFalse()
            {
                // Act
                var result = _threadService.GetThreadsByCharacter("98765", false, false, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(2);
                result[1].Should().HaveCount(2);
                result[2].Should().HaveCount(2);
            }

            [Fact]
            public void ReturnsAllNonHiatusedThreadsWhenIncludeArchivedIsTrueAndIncludeHiatusedIsFalse()
            {
                // Act
                var result = _threadService.GetThreadsByCharacter("98765", true, false, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(2);
                result[1].Should().HaveCount(3);
                result[2].Should().HaveCount(3);
            }

            [Fact]
            public void ReturnsAllActiveThreadsWhenIncludeArchivedIsFalseAndIncludeHiatusedIsTrue()
            {
                // Act
                var result = _threadService.GetThreadsByCharacter("98765", false, true, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(3);
                result[1].Should().HaveCount(2);
                result[2].Should().HaveCount(2);
                result[3].Should().HaveCount(2);
            }

            [Fact]
            public void ReturnsAllThreadsWhenIncludeArchivedIsTrueAndIncludeHiatusedIsTrue()
            {
                // Act
                var result = _threadService.GetThreadsByCharacter("98765", true, true, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(3);
                result[1].Should().HaveCount(3);
                result[2].Should().HaveCount(3);
                result[3].Should().HaveCount(3);
            }
        }
    }
}
