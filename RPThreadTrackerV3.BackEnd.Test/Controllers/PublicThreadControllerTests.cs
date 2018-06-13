// <copyright file="PublicThreadControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using BackEnd.Controllers;
    using BackEnd.Infrastructure.Exceptions.PublicViews;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.DomainModels.PublicViews;
    using BackEnd.Models.ViewModels;
    using BackEnd.Models.ViewModels.PublicViews;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TestHelpers;
    using Xunit;
    using Documents = BackEnd.Infrastructure.Data.Documents;
    using Entities = BackEnd.Infrastructure.Data.Entities;

    [Trait("Class", "PublicThreadController")]
    public class PublicThreadControllerTests : ControllerTests<PublicThreadController>
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IThreadService> _mockThreadService;
        private readonly Mock<IRepository<Entities.Thread>> _mockThreadRepository;
        private readonly Mock<IPublicViewService> _mockPublicViewService;
        private readonly Mock<IDocumentRepository<Documents.PublicView>> _mockPublicViewRepository;

        public PublicThreadControllerTests()
        {
            var mockLogger = new Mock<ILogger<PublicThreadController>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<PublicViewDto>(It.IsAny<PublicView>()))
                .Returns((PublicView model) => new PublicViewDto
                {
                    UserId = model.UserId,
                    Name = model.Name,
                    Id = model.Id,
                    Slug = model.Slug
                });
            _mockMapper.Setup(m => m.Map<PublicView>(It.IsAny<PublicViewDto>()))
                .Returns((PublicViewDto dto) => new PublicView
                {
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Id = dto.Id,
                    Slug = dto.Slug
                });
            _mockMapper.Setup(m => m.Map<ThreadDto>(It.IsAny<Thread>()))
                .Returns((Thread model) => new ThreadDto
                {
                    ThreadId = model.ThreadId,
                    CharacterId = model.CharacterId,
                    PartnerUrlIdentifier = model.PartnerUrlIdentifier,
                    PostId = model.PostId
                });
            _mockMapper.Setup(m => m.Map<Thread>(It.IsAny<ThreadDto>()))
                .Returns((ThreadDto dto) => new Thread
                {
                    ThreadId = dto.ThreadId.GetValueOrDefault(),
                    CharacterId = dto.CharacterId,
                    PartnerUrlIdentifier = dto.PartnerUrlIdentifier,
                    PostId = dto.PostId
                });
            _mockMapper.Setup(m => m.Map<List<ThreadDto>>(It.IsAny<List<Thread>>()))
                .Returns((List<Thread> models) => models.Select(m => _mockMapper.Object.Map<ThreadDto>(m)).ToList());
            _mockThreadService = new Mock<IThreadService>();
            _mockThreadRepository = new Mock<IRepository<Entities.Thread>>();
            _mockPublicViewService = new Mock<IPublicViewService>();
            _mockPublicViewRepository = new Mock<IDocumentRepository<Documents.PublicView>>();
            var mockCharacterService = new Mock<ICharacterService>();
            var mockCharacterRepository = new Mock<IRepository<Entities.Character>>();
            Controller = new PublicThreadController(mockLogger.Object, _mockMapper.Object, _mockThreadService.Object, _mockThreadRepository.Object, _mockPublicViewService.Object, _mockPublicViewRepository.Object, mockCharacterService.Object, mockCharacterRepository.Object);
            InitControllerContext();
        }

        public class GetPublicThreads : PublicThreadControllerTests
        {
            [Fact]
            public async Task ReturnsNotFoundWhenPublicViewCannotBeFound()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.GetViewBySlug("my-view", _mockPublicViewRepository.Object, _mockMapper.Object)).Throws<PublicViewNotFoundException>();

                // Act
                var result = await Controller.Get("my-view");

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.GetViewBySlug("my-view", _mockPublicViewRepository.Object, _mockMapper.Object)).Throws<NullReferenceException>();

                // Act
                var result = await Controller.Get("my-view");

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWithThreadsWhenRequestSuccessful()
            {
                // Arrange
                var view = new PublicView
                {
                    Id = "13579",
                    Slug = "my-view"
                };
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
                _mockPublicViewService.Setup(s =>
                        s.GetViewBySlug("my-view", _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Returns(Task.FromResult(view));
                _mockThreadService.Setup(s =>
                        s.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns(threads);

                // Act
                var result = await Controller.Get("my-view");
                var body = ((OkObjectResult)result).Value as PublicThreadDtoCollection;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.View.Id.Should().Be("13579");
                body.Threads.Should().HaveCount(2);
                body.Threads.Should().Contain(t => t.ThreadId == 12345);
                body.Threads.Should().Contain(t => t.ThreadId == 54321);
            }
        }

        public class Post : PublicThreadControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                var request = new LegacyPublicViewDto { CharacterUrlIdentifier = "my-character", UserId = "12345" };
                _mockThreadService.Setup(s =>
                        s.GetThreadsForView(It.IsAny<PublicView>(), _mockThreadRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                #pragma warning disable CS0618 // Type or member is obsolete
                var result = Controller.Post(request);
                #pragma warning restore CS0618 // Type or member is obsolete

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWithThreadsWhenRequestSuccessful()
            {
                // Arrange
                var view = new PublicView
                {
                    Id = "13579",
                    Slug = "my-view"
                };
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
                var request = new LegacyPublicViewDto { CharacterUrlIdentifier = "my-character", UserId = "12345" };
                _mockPublicViewService.Setup(s =>
                        #pragma warning disable CS0618 // Type or member is obsolete
                        s.GetViewFromLegacyDto(request, It.IsAny<IEnumerable<Character>>()))
                        #pragma warning restore CS0618 // Type or member is obsolete
                    .Returns(view);
                _mockThreadService.Setup(s =>
                        s.GetThreadsForView(view, _mockThreadRepository.Object, _mockMapper.Object))
                    .Returns(threads);

                // Act
                #pragma warning disable CS0618 // Type or member is obsolete
                var result = Controller.Post(request);
                #pragma warning restore CS0618 // Type or member is obsolete
                var body = ((OkObjectResult)result).Value as PublicThreadDtoCollection;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.View.Id.Should().Be("13579");
                body.Threads.Should().HaveCount(2);
                body.Threads.Should().Contain(t => t.ThreadId == 12345);
                body.Threads.Should().Contain(t => t.ThreadId == 54321);
            }
        }
    }
}
