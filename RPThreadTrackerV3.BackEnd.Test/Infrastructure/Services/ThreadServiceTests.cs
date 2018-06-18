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
    using BackEnd.Infrastructure.Exceptions.Thread;
    using BackEnd.Infrastructure.Services;
    using BackEnd.Models.DomainModels.PublicViews;
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
                    IsArchived = entity.IsArchived,
                    ThreadTags = entity.ThreadTags?.Select(t => new DomainModels.ThreadTag
                    {
                        TagText = t.TagText,
                        ThreadId = t.ThreadId,
                        ThreadTagId = t.ThreadTagId
                    }).ToList(),
                    Character = new DomainModels.Character
                    {
                        CharacterId = entity.Character.CharacterId,
                        CharacterName = entity.Character?.CharacterName
                    },
                    DateMarkedQueued = entity.DateMarkedQueued,
                    PartnerUrlIdentifier = entity.PartnerUrlIdentifier,
                    PostId = entity.PostId
                });
            _mockMapper.Setup(m => m.Map<Thread>(It.IsAny<DomainModels.Thread>()))
                .Returns((DomainModels.Thread model) => new Thread
                {
                    ThreadId = model.ThreadId,
                    CharacterId = model.CharacterId,
                    UserTitle = model.UserTitle,
                    IsArchived = model.IsArchived,
                    ThreadTags = model.ThreadTags?.Select(t => new ThreadTag
                    {
                        TagText = t.TagText,
                        ThreadId = t.ThreadId,
                        ThreadTagId = t.ThreadTagId,
                        Thread = new Thread()
                    }).ToList(),
                    Character = new Character
                        {
                        CharacterId = model.Character?.CharacterId ?? 0,
                        CharacterName = model.Character?.CharacterName
                    },
                    DateMarkedQueued = model.DateMarkedQueued,
                    PartnerUrlIdentifier = model.PartnerUrlIdentifier,
                    PostId = model.PostId
                });
            _mockMapper.Setup(m => m.Map<List<DomainModels.Thread>>(It.IsAny<List<Thread>>()))
                .Returns((List<Thread> models) => models.Select(m => _mockMapper.Object.Map<DomainModels.Thread>(m)).ToList());
            var threadList = BuildThreadList();
            _mockThreadRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Thread, bool>>>(y => threadList.Any(y.Compile())), It.IsAny<List<string>>())).Returns((Expression<Func<Thread, bool>> predicate, List<string> navigationProperties) => threadList.Where(predicate.Compile()));
            _threadService = new ThreadService();
        }

        private List<Thread> BuildThreadListWithTags()
        {
            var tag1 = new ThreadTag
            {
                TagText = "Tag1",
                ThreadTagId = "ThreadTag1",
                ThreadId = 1,
                Thread = new Thread()
            };
            var tag2 = new ThreadTag
            {
                TagText = "Tag2",
                ThreadTagId = "ThreadTag2",
                ThreadId = 2,
                Thread = new Thread()
            };
            var tag3 = new ThreadTag
            {
                TagText = "Tag3",
                ThreadTagId = "ThreadTag3",
                ThreadId = 3,
                Thread = new Thread()
            };
            var tag4 = new ThreadTag
            {
                TagText = "Tag4",
                ThreadTagId = "ThreadTag4",
                ThreadId = 4,
                Thread = new Thread()
            };
            var thread1 = new Thread
            {
                Character = new Character { UserId = "12345" },
                ThreadId = 1,
                ThreadTags = new List<ThreadTag>
                    {
                        tag1,
                        tag2,
                    }
            };
            var thread2 = new Thread()
            {
                Character = new Character { UserId = "12345" },
                ThreadId = 2,
                ThreadTags = new List<ThreadTag>
                    {
                        tag2,
                        tag3
                    }
            };
            var thread3 = new Thread()
            {
                Character = new Character { UserId = "12345" },
                ThreadId = 3,
                ThreadTags = new List<ThreadTag>
                    {
                        tag1,
                        tag3
                    }
            };
            var anotherUsersThread = new Thread
            {
                Character = new Character { UserId = "23456" },
                ThreadId = 4,
                ThreadTags = new List<ThreadTag>
                    {
                        tag2,
                        tag4
                    }
            };
            return new List<Thread>
                {
                    thread1,
                    thread2,
                    thread3,
                    anotherUsersThread
                };
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

        public class AssertUserOwnsThread : ThreadServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfThreadDoesNotExistForUser()
            {
                // Arrange
                var character = new Character
                {
                    UserId = "54321",
                    CharacterId = 13579
                };
                var thread = new Thread
                {
                    CharacterId = 13579,
                    ThreadId = 97531,
                    Character = character
                };
                _mockThreadRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Thread, bool>>>(y => y.Compile()(thread)))).Returns(true);
                _mockThreadRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Thread, bool>>>(y => !y.Compile()(thread)))).Returns(false);

                // Act
                Action action = () => _threadService.AssertUserOwnsThread(97531, "12345", _mockThreadRepository.Object);

                // Assert
                action.Should().Throw<ThreadNotFoundException>();
            }

            [Fact]
            public void ThrowsNoExceptionIfThreadExistsForUser()
            {
                // Arrange
                var character = new Character
                {
                    UserId = "12345",
                    CharacterId = 13579
                };
                var thread = new Thread
                {
                    CharacterId = 13579,
                    ThreadId = 97531,
                    Character = character
                };
                _mockThreadRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Thread, bool>>>(y => y.Compile()(thread)))).Returns(true);
                _mockThreadRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Thread, bool>>>(y => !y.Compile()(thread)))).Returns(false);

                // Act
                _threadService.AssertUserOwnsThread(97531, "12345", _mockThreadRepository.Object);

                // Assert
                _mockThreadRepository.Verify(r => r.ExistsWhere(It.Is<Expression<Func<Thread, bool>>>(y => y.Compile()(thread))), Times.Once);
            }
        }

        public class UpdateThread : ThreadServiceTests
        {
            [Fact]
            public void UpdatesThreadInRepository()
            {
                // Arrange
                var thread = new DomainModels.Thread
                {
                    CharacterId = 13579,
                    ThreadId = 12345,
                    UserTitle = "Test Thread",
                    IsArchived = true
                };
                _mockThreadRepository.Setup(r => r.Update("12345", It.IsAny<Thread>())).Returns((string threadId, Thread entity) => entity);

                // Act
                var result = _threadService.UpdateThread(thread, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                _mockThreadRepository.Verify(r => r.Update("12345", It.Is<Thread>(t => t.CharacterId == 13579 && t.ThreadId == 12345 && t.UserTitle == "Test Thread" && t.IsArchived)), Times.Once);
                result.ThreadId.Should().Be(12345);
                result.UserTitle.Should().Be("Test Thread");
                result.CharacterId.Should().Be(13579);
                result.IsArchived.Should().Be(true);
            }
        }

        public class DeleteThread : ThreadServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfThreadDoesNotExist()
            {
                // Arrange
                _mockThreadRepository.Setup(r => r.GetWhere(It.IsAny<Expression<Func<Thread, bool>>>(), It.IsAny<List<string>>())).Returns(new List<Thread>());

                // Act
                Action action = () => _threadService.DeleteThread(13579, _mockThreadRepository.Object);

                // Assert
                action.Should().Throw<ThreadNotFoundException>();
            }

            [Fact]
            public void DeletesThreadIfExists()
            {
                // Arrange
                var thread = new Thread
                {
                    ThreadId = 13579,
                    UserTitle = "Test Thread"
                };
                _mockThreadRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Thread, bool>>>(y => y.Compile()(thread)), It.IsAny<List<string>>())).Returns(new List<Thread> { thread });

                // Act
                _threadService.DeleteThread(13579, _mockThreadRepository.Object);

                // Assert
                _mockThreadRepository.Verify(r => r.Delete(thread), Times.Once);
            }
        }

        public class CreateThread : ThreadServiceTests
        {
            [Fact]
            public void InsertsNewThreadInRepository()
            {
                // Arrange
                var thread = new DomainModels.Thread
                {
                    CharacterId = 12345,
                    UserTitle = "Test Thread"
                };
                _mockThreadRepository.Setup(r => r.Create(It.IsAny<Thread>())).Returns((Thread entity) => entity);

                // Act
                var result = _threadService.CreateThread(thread, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                _mockThreadRepository.Verify(r => r.Create(It.Is<Thread>(t => t.UserTitle == "Test Thread" && t.CharacterId == 12345)), Times.Once);
                result.CharacterId.Should().Be(12345);
                result.UserTitle.Should().Be("Test Thread");
            }
        }

        public class GetAllTags : ThreadServiceTests
        {
            [Fact]
            public void GetsDeduplicatedListOfTagsBelongingToUser()
            {
                // Arrange
                var threadList = BuildThreadListWithTags();
                _mockThreadRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Thread, bool>>>(y => threadList.Any(y.Compile())), It.IsAny<List<string>>())).Returns((Expression<Func<Thread, bool>> predicate, List<string> navigationProperties) => threadList.Where(predicate.Compile()));

                // Act
                var tags = _threadService.GetAllTags("12345", _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                tags.Should().HaveCount(3)
                    .And.Contain("Tag1")
                    .And.Contain("Tag2")
                    .And.Contain("Tag3");
            }

            [Fact]
            public void ReturnsEmptyListWhenNoTagsFoundForUser()
            {
                // Arrange
                var threadList = BuildThreadListWithTags();
                threadList.ForEach(t => t.ThreadTags = new List<ThreadTag>());
                _mockThreadRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Thread, bool>>>(y => threadList.Any(y.Compile())), It.IsAny<List<string>>())).Returns((Expression<Func<Thread, bool>> predicate, List<string> navigationProperties) => threadList.Where(predicate.Compile()));

                // Act
                var tags = _threadService.GetAllTags("12345", _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                tags.Should().HaveCount(0);
            }
        }

        public class GetThreadsForView : ThreadServiceTests
        {
            public GetThreadsForView()
            {
                var filterableThreads = new List<Thread>();
                var character1 = new Character
                {
                    CharacterId = 1,
                    CharacterName = "Character 1"
                };
                var character2 = new Character
                {
                    CharacterId = 2,
                    CharacterName = "Character 2"
                };
                filterableThreads.Add(new Thread
                {
                    ThreadId = 1,
                    Character = character1,
                    CharacterId = 1,
                    IsArchived = false,
                    DateMarkedQueued = null,
                    ThreadTags = new List<ThreadTag> { new ThreadTag { TagText = "Tag1" } },
                    PartnerUrlIdentifier = "my-partner",
                    PostId = "1",
                    UserTitle = "Active Thread 1 Character 1"
                });
                filterableThreads.Add(new Thread
                {
                    ThreadId = 2,
                    Character = character1,
                    CharacterId = 1,
                    IsArchived = false,
                    DateMarkedQueued = null,
                    ThreadTags = new List<ThreadTag> { new ThreadTag { TagText = "Tag2" } },
                    PartnerUrlIdentifier = "my-other-partner",
                    PostId = "3",
                    UserTitle = "Active Thread 2 Character 1"
                });
                filterableThreads.Add(new Thread
                {
                    ThreadId = 3,
                    Character = character2,
                    CharacterId = 2,
                    IsArchived = false,
                    DateMarkedQueued = null,
                    ThreadTags = new List<ThreadTag> { new ThreadTag { TagText = "Tag1" } },
                    PartnerUrlIdentifier = "my-other-partner",
                    PostId = "3",
                    UserTitle = "Active Thread 3 Character 2"
                });
                filterableThreads.Add(new Thread
                {
                    ThreadId = 4,
                    Character = character2,
                    CharacterId = 2,
                    IsArchived = false,
                    DateMarkedQueued = null,
                    ThreadTags = new List<ThreadTag> { new ThreadTag { TagText = "Tag2" } },
                    PartnerUrlIdentifier = "my-other-partner",
                    PostId = "4",
                    UserTitle = "Active Thread 4 Character 2"
                });
                filterableThreads.Add(new Thread
                {
                    ThreadId = 5,
                    Character = character1,
                    CharacterId = 1,
                    IsArchived = true,
                    DateMarkedQueued = null,
                    ThreadTags = new List<ThreadTag> { new ThreadTag { TagText = "Tag3" } },
                    PartnerUrlIdentifier = "my-partner",
                    PostId = "5",
                    UserTitle = "Archived Thread Character 1"
                });
                filterableThreads.Add(new Thread
                {
                    ThreadId = 6,
                    Character = character2,
                    CharacterId = 2,
                    IsArchived = true,
                    DateMarkedQueued = null,
                    ThreadTags = new List<ThreadTag> { new ThreadTag { TagText = "Tag3" } },
                    PartnerUrlIdentifier = "my-partner",
                    PostId = "6",
                    UserTitle = "Archived Thread Character 2"
                });
                _mockThreadRepository.Setup(r =>
                        r.GetWhere(It.IsAny<Expression<Func<Thread, bool>>>(), It.IsAny<List<string>>()))
                    .Returns(filterableThreads);
            }

            [Fact]
            public void IncludesArchivedThreadsWhenTurnFilterIncludesArchived()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeArchived = true
                    },
                    CharacterIds = new List<int>(),
                    Tags = new List<string>()
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(2);
            }

            [Fact]
            public void IncludesAllActiveThreadsWhenTurnFilterIncludesMyTurn()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeMyTurn = true
                    },
                    CharacterIds = new List<int>(),
                    Tags = new List<string>()
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(4);
            }

            [Fact]
            public void IncludesAllActiveThreadsWhenTurnFilterIncludesTheirTurn()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeTheirTurn = true
                    },
                    CharacterIds = new List<int>(),
                    Tags = new List<string>()
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(4);
            }

            [Fact]
            public void IncludesAllActiveThreadsWhenTurnFilterIncludesQueued()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeQueued = true
                    },
                    CharacterIds = new List<int>(),
                    Tags = new List<string>()
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(4);
            }

            [Fact]
            public void IncludesAllThreadsWhenTurnFilterIncludesArchivedAndActive()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeQueued = true,
                        IncludeArchived = true
                    },
                    CharacterIds = new List<int>(),
                    Tags = new List<string>()
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(6);
            }

            [Fact]
            public void FiltersByCharacterWhenCharacterIdProvided()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeQueued = true,
                        IncludeArchived = true
                    },
                    CharacterIds = new List<int> { 1 },
                    Tags = new List<string>()
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(3);
            }

            [Fact]
            public void FiltersByTagWhenTagsProvided()
            {
                // Arrange
                var view = new PublicView
                {
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeQueued = true,
                        IncludeArchived = true
                    },
                    CharacterIds = new List<int>(),
                    Tags = new List<string> { "Tag2", "Tag3" }
                };

                // Act
                var result = _threadService.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object);

                // Assert
                result.Should().HaveCount(4);
            }
        }
    }
}
