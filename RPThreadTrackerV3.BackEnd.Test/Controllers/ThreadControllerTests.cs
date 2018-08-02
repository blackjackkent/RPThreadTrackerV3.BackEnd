// <copyright file="ThreadControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using BackEnd.Controllers;
    using BackEnd.Infrastructure.Exceptions.Characters;
    using BackEnd.Infrastructure.Exceptions.Thread;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OfficeOpenXml;
    using TestHelpers;
    using Xunit;
    using Entities = BackEnd.Infrastructure.Data.Entities;

    [Trait("Class", "ThreadController")]
    public class ThreadControllerTests : ControllerTests<ThreadController>
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IThreadService> _mockThreadService;
        private readonly Mock<IRepository<Entities.Thread>> _mockThreadRepository;
        private readonly Mock<IExporterService> _mockExporterService;
        private readonly Mock<ICharacterService> _mockCharacterService;
        private readonly Mock<IRepository<Entities.Character>> _mockCharacterRepository;

        public ThreadControllerTests()
        {
            var mockLogger = new Mock<ILogger<ThreadController>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<ThreadDto>(It.IsAny<Thread>()))
                .Returns((Thread model) => new ThreadDto
                {
                    ThreadId = model.ThreadId,
                    CharacterId = model.CharacterId,
                    PartnerUrlIdentifier = model.PartnerUrlIdentifier,
                    PostId = model.PostId,
                    UserTitle = model.UserTitle,
                    Character = new CharacterDto
                    {
                        CharacterId = model.Character?.CharacterId ?? 0,
                        CharacterName = model.Character?.CharacterName
                    },
                    ThreadTags = model.ThreadTags?.Select(t => new ThreadTagDto
                    {
                        TagText = t.TagText,
                        ThreadId = t.ThreadId,
                        ThreadTagId = t.ThreadTagId
                    }).ToList(),
                    IsArchived = model.IsArchived,
                    DateMarkedQueued = model.DateMarkedQueued,
                    Description = model.Description
                });
            _mockMapper.Setup(m => m.Map<Thread>(It.IsAny<ThreadDto>()))
                .Returns((ThreadDto dto) => new Thread
                {
                    ThreadId = dto.ThreadId.GetValueOrDefault(),
                    CharacterId = dto.CharacterId,
                    PartnerUrlIdentifier = dto.PartnerUrlIdentifier,
                    PostId = dto.PostId,
                    UserTitle = dto.UserTitle,
                    Character = new Character
                    {
                        CharacterId = dto.Character?.CharacterId ?? 0,
                        CharacterName = dto.Character?.CharacterName
                    },
                    ThreadTags = dto.ThreadTags?.Select(t => new ThreadTag
                    {
                        ThreadTagId = t.ThreadTagId,
                        ThreadId = t.ThreadId,
                        TagText = t.TagText
                    }).ToList(),
                    IsArchived = dto.IsArchived,
                    DateMarkedQueued = dto.DateMarkedQueued,
                    Description = dto.Description
                });
            _mockMapper.Setup(m => m.Map<List<ThreadDto>>(It.IsAny<List<Thread>>()))
                .Returns((List<Thread> models) => models.Select(m => _mockMapper.Object.Map<ThreadDto>(m)).ToList());
            _mockThreadService = new Mock<IThreadService>();
            _mockThreadRepository = new Mock<IRepository<Entities.Thread>>();
            _mockCharacterService = new Mock<ICharacterService>();
            _mockCharacterRepository = new Mock<IRepository<Entities.Character>>();
            _mockExporterService = new Mock<IExporterService>();
            Controller = new ThreadController(mockLogger.Object, _mockMapper.Object, _mockThreadService.Object, _mockThreadRepository.Object, _mockCharacterService.Object, _mockCharacterRepository.Object, _mockExporterService.Object);
            InitControllerContext();
        }

        public class GetThreads : ThreadControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockThreadService.Setup(s => s.GetThreads("12345", It.IsAny<bool>(), _mockThreadRepository.Object, _mockMapper.Object)).Throws<NullReferenceException>();

                // Act
                var result = Controller.Get(true);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWithThreadsWhenRequestSuccessful()
            {
                // Arrange
                var threads = new List<Thread>
                {
                    new Thread
                    {
                        ThreadId = 12345
                    },
                    new Thread
                    {
                        ThreadId = 54321
                    }
                };
                _mockThreadService.Setup(s =>
                        s.GetThreads("12345", true, _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns(threads);

                // Act
                var result = Controller.Get(true);
                var body = ((OkObjectResult)result).Value as ThreadDtoCollection;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body?.Threads.Should().HaveCount(2)
                    .And.Contain(t => t.ThreadId == 12345)
                    .And.Contain(t => t.ThreadId == 54321);
            }
        }

        public class Post : ThreadControllerTests
        {
            private readonly ThreadDto _validRequest;

            public Post()
            {
                _validRequest = new ThreadDto
                {
                    UserTitle = "My Thread",
                    CharacterId = 54321
                };
            }

            [Fact]
            public void ReturnsBadRequestWhenThreadIsInvalid()
            {
                // Arrange
                var thread = new Mock<ThreadDto>();
                thread.Setup(c => c.AssertIsValid()).Throws<InvalidThreadException>();

                // Act
                var result = Controller.Post(thread.Object);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockThreadService.Verify(s => s.CreateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsBadRequestWhenCharacterDoesNotExistForUser()
            {
                // Arrange
                _mockCharacterService
                    .Setup(s => s.AssertUserOwnsCharacter(54321, "12345", _mockCharacterRepository.Object))
                    .Throws<CharacterNotFoundException>();

                // Act
                var result = Controller.Post(_validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockThreadService.Verify(s => s.CreateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockThreadService.Setup(s => s.CreateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Post(_validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                _mockThreadService.Setup(s =>
                        s.CreateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns((Thread model, IRepository<Entities.Thread> repo, IMapper mapper) => model);

                // Act
                var result = Controller.Post(_validRequest);
                var body = ((OkObjectResult)result).Value as ThreadDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body?.UserTitle.Should().Be("My Thread");
            }
        }

        public class Put : ThreadControllerTests
        {
            private readonly ThreadDto _validRequest;

            public Put()
            {
                _validRequest = new ThreadDto
                {
                    ThreadId = 13579,
                    CharacterId = 54321,
                    UserTitle = "My Thread",
                    Character = new CharacterDto { CharacterId = 54321 },
                    ThreadTags = new List<ThreadTagDto>
                    {
                        new ThreadTagDto
                        {
                            ThreadId = 13579,
                            ThreadTagId = "Tag1",
                            TagText = "Test Tag"
                        }
                    },
                    DateMarkedQueued = DateTime.UtcNow,
                    Description = "Test Description",
                    IsArchived = false,
                    PartnerUrlIdentifier = "test-partner",
                    PostId = "12345",
                    ThreadHomeUrl = "http://www.blah.com"
                };
            }

            [Fact]
            public void ReturnsBadRequestWhenThreadIsInvalid()
            {
                // Arrange
                var thread = new Mock<ThreadDto>();
                thread.Setup(c => c.AssertIsValid()).Throws<InvalidThreadException>();

                // Act
                var result = Controller.Put(13579, thread.Object);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockThreadService.Verify(s => s.UpdateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsBadRequestWhenCharacterDoesNotExistForUser()
            {
                // Arrange
                _mockCharacterService
                    .Setup(s => s.AssertUserOwnsCharacter(54321, "12345", _mockCharacterRepository.Object))
                    .Throws<CharacterNotFoundException>();

                // Act
                var result = Controller.Put(13579, _validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockThreadService.Verify(s => s.UpdateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsBadRequestWhenThreadDoesNotExistForUser()
            {
                // Arrange
                _mockThreadService
                    .Setup(s => s.AssertUserOwnsThread(13579, "12345", _mockThreadRepository.Object))
                    .Throws<ThreadNotFoundException>();

                // Act
                var result = Controller.Put(13579, _validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockThreadService.Verify(s => s.UpdateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockThreadService.Setup(s => s.UpdateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Put(13579, _validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                _mockThreadService.Setup(s =>
                        s.UpdateThread(It.IsAny<Thread>(), _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns((Thread model, IRepository<Entities.Thread> repo, IMapper mapper) => model);

                // Act
                var result = Controller.Put(13579, _validRequest);
                var body = ((OkObjectResult)result).Value as ThreadDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body?.ThreadId.Should().Be(13579);
                body?.CharacterId.Should().Be(54321);
                body?.UserTitle.Should().Be("My Thread");
            }
        }

        public class Delete : ThreadControllerTests
        {
            [Fact]
            public void ReturnsNotFoundWhenThreadDoesNotExistForUser()
            {
                // Arrange
                _mockThreadService
                    .Setup(s => s.AssertUserOwnsThread(13579, "12345", _mockThreadRepository.Object))
                    .Throws<ThreadNotFoundException>();

                // Act
                var result = Controller.Delete(13579);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                _mockThreadService.Verify(s => s.DeleteThread(It.IsAny<int>(), _mockThreadRepository.Object), Times.Never);
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockThreadService.Setup(s => s.DeleteThread(13579, _mockThreadRepository.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Delete(13579);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Act
                var result = Controller.Delete(13579);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }

        public class Export : ThreadControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockExporterService.Setup(s => s.GetExcelPackage(It.IsAny<IEnumerable<Character>>(), It.IsAny<Dictionary<int, List<Thread>>>()))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Export(true, true);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsFileDataWhenRequestSuccessful()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character
                    {
                        CharacterId = 12345
                    },
                    new Character
                    {
                        CharacterId = 54321
                    }
                };
                var threads = new Dictionary<int, List<Thread>>
                {
                    {
                        12345,
                        new List<Thread>
                        {
                            new Thread { CharacterId = 12345, ThreadId = 13579 },
                            new Thread { CharacterId = 12345, ThreadId = 97531 }
                        }
                    },
                    {
                        54321,
                        new List<Thread>()
                        {
                            new Thread { CharacterId = 54321, ThreadId = 24680 },
                            new Thread { CharacterId = 54321, ThreadId = 08642 }
                        }
                    }
                };
                _mockCharacterService
                    .Setup(s => s.GetCharacters("12345", _mockCharacterRepository.Object, _mockMapper.Object, true))
                    .Returns(characters);
                _mockThreadService.Setup(s =>
                        s.GetThreadsByCharacter("12345", true, true, _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns(threads);
                var mockPackage = GetMockPackage();
                _mockExporterService.Setup(s => s.GetExcelPackage(characters, threads)).Returns(mockPackage);

                // Act
                var result = Controller.Export(true, true);

                // Assert
                result.Should().BeOfType<FileContentResult>()
                    .Which.FileContents.Length.Should().Be(GetMockPackage().GetAsByteArray().Length);
            }

            private ExcelPackage GetMockPackage()
            {
                var mockPackage = new ExcelPackage { File = new FileInfo("test file") };
                mockPackage.Workbook.Worksheets.Add("My Character");
                return mockPackage;
            }
        }

        public class Tags : ThreadControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockThreadService.Setup(s => s.GetAllTags("12345", _mockThreadRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Tags();

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkResponseWithTagsWhenRequestSuccessful()
            {
                // Arrange
                var tags = new List<string> { "tag1", "tag2", "tag3" };
                _mockThreadService.Setup(s => s.GetAllTags("12345", _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns(tags);

                // Act
                var result = Controller.Tags();
                var body = ((OkObjectResult)result).Value as List<string>;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.Should().HaveCount(3);
            }
        }
    }
}
