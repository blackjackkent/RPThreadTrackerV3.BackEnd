// <copyright file="PublicViewManagementControllerTests.cs" company="Rosalind Wills">
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
    using BackEnd.Models.DomainModels.PublicViews;
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

    [Trait("Class", "PublicViewManagementController")]
    public class PublicViewManagementControllerTests : ControllerTests<PublicViewManagementController>
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPublicViewService> _mockPublicViewService;
        private readonly Mock<IDocumentRepository<Documents.PublicView>> _mockPublicViewRepository;

        public PublicViewManagementControllerTests()
        {
            var mockLogger = new Mock<ILogger<PublicViewManagementController>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<PublicViewDto>(It.IsAny<PublicView>()))
                .Returns((PublicView model) => new PublicViewDto
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    Name = model.Name,
                    Slug = model.Slug,
                    TurnFilter = new PublicTurnFilterDto
                    {
                        IncludeMyTurn = model.TurnFilter?.IncludeMyTurn ?? false,
                        IncludeArchived = model.TurnFilter?.IncludeArchived ?? false,
                        IncludeQueued = model.TurnFilter?.IncludeQueued ?? false,
                        IncludeTheirTurn = model.TurnFilter?.IncludeTheirTurn ?? false
                    },
                    Columns = model.Columns,
                    CharacterIds = model.CharacterIds,
                    SortKey = model.SortKey,
                    Tags = model.Tags,
                    SortDescending = model.SortDescending
                });
            _mockMapper.Setup(m => m.Map<PublicView>(It.IsAny<PublicViewDto>()))
                .Returns((PublicViewDto dto) => new PublicView
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Slug = dto.Slug,
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeMyTurn = dto.TurnFilter?.IncludeMyTurn ?? false,
                        IncludeArchived = dto.TurnFilter?.IncludeArchived ?? false,
                        IncludeQueued = dto.TurnFilter?.IncludeQueued ?? false,
                        IncludeTheirTurn = dto.TurnFilter?.IncludeTheirTurn ?? false
                    },
                    Columns = dto.Columns,
                    CharacterIds = dto.CharacterIds,
                    SortKey = dto.SortKey,
                    Tags = dto.Tags,
                    SortDescending = dto.SortDescending
                });
            _mockPublicViewService = new Mock<IPublicViewService>();
            _mockPublicViewRepository = new Mock<IDocumentRepository<Documents.PublicView>>();
            Controller = new PublicViewManagementController(mockLogger.Object, _mockMapper.Object, _mockPublicViewService.Object, _mockPublicViewRepository.Object);
            InitControllerContext();
        }

        public class GetPublicViews : PublicViewManagementControllerTests
        {
            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.GetPublicViews("12345", _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Get();

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWithViewListWhenRequestSuccessful()
            {
                // Arrange
                var views = new List<PublicView>
                {
                    new PublicView
                    {
                        Id = "13579"
                    },
                    new PublicView
                    {
                        Id = "97531"
                    }
                };
                _mockPublicViewService.Setup(s => s.GetPublicViews("12345", _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Returns(Task.FromResult(views.AsEnumerable()));

                // Act
                var result = await Controller.Get();
                var body = ((OkObjectResult)result).Value as List<PublicViewDto>;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.Should().HaveCount(2)
                    .And.Contain(v => v.Id == "13579")
                    .And.Contain(v => v.Id == "97531");
            }
        }

        public class Post : PublicViewManagementControllerTests
        {
            private readonly PublicViewDto _validRequest;

            public Post()
            {
                _validRequest = new PublicViewDto
                {
                    Name = "My View",
                    Slug = "my-view",
                    Columns = new List<string> { "column1", "column2" },
                    CharacterIds = new List<int> { 1, 2, 3 },
                    TurnFilter = new PublicTurnFilterDto { IncludeMyTurn = true }
                };
            }

            [Fact]
            public async Task ReturnsBadRequestWhenViewIsInvalid()
            {
                // Arrange
                var view = new Mock<PublicViewDto>();
                view.Setup(c => c.AssertIsValid()).Throws<InvalidPublicViewException>();

                // Act
                var result = await Controller.Post(view.Object);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockPublicViewService.Verify(s => s.CreatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenViewSlugExists()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.CreatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Throws<PublicViewSlugExistsException>();

                // Act
                var result = await Controller.Post(_validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.CreatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Post(_validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                _mockPublicViewService.Setup(s =>
                        s.CreatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Returns((PublicView model, IDocumentRepository<Documents.PublicView> repo, IMapper mapper) => Task.FromResult(model));

                // Act
                var result = await Controller.Post(_validRequest);
                var body = ((OkObjectResult)result).Value as PublicViewDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.Name.Should().Be("My View");
            }

            [Fact]
            public async Task SetsUserIdWhenCreatingView()
            {
                // Arrange
                _mockPublicViewService.Setup(s =>
                        s.CreatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Returns((PublicView model, IDocumentRepository<Documents.PublicView> repo, IMapper mapper) => Task.FromResult(model));

                // Act
                await Controller.Post(_validRequest);

                // Assert
                _mockPublicViewService.Verify(s => s.CreatePublicView(It.Is<PublicView>(v => v.UserId == "12345"), _mockPublicViewRepository.Object, _mockMapper.Object));
            }
        }

        public class Put : PublicViewManagementControllerTests
        {
            private readonly PublicViewDto _validRequest;

            public Put()
            {
                _validRequest = new PublicViewDto
                {
                    Id = "13579",
                    Name = "My View",
                    Slug = "my-view",
                    Columns = new List<string> { "column1", "column2" },
                    CharacterIds = new List<int> { 1, 2, 3 },
                    TurnFilter = new PublicTurnFilterDto { IncludeMyTurn = true }
                };
            }

            [Fact]
            public async Task ReturnsBadRequestWhenViewIsInvalid()
            {
                // Arrange
                var view = new Mock<PublicViewDto>();
                view.Setup(c => c.AssertIsValid()).Throws<InvalidPublicViewException>();

                // Act
                var result = await Controller.Put("13579", view.Object);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockPublicViewService.Verify(s => s.UpdatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsBadRequestWhenViewSlugExists()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.UpdatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Throws<PublicViewSlugExistsException>();

                // Act
                var result = await Controller.Put("13579", _validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task ReturnsBadRequestWhenViewDoesNotExistForUser()
            {
                // Arrange
                _mockPublicViewService
                    .Setup(s => s.AssertUserOwnsPublicView("13579", "12345", _mockPublicViewRepository.Object))
                    .Throws<PublicViewNotFoundException>();

                // Act
                var result = await Controller.Put("13579", _validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockPublicViewService.Verify(s => s.UpdatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.UpdatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Put("13579", _validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                _mockPublicViewService.Setup(s =>
                        s.UpdatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Returns((PublicView model, IDocumentRepository<Documents.PublicView> repo, IMapper mapper) => Task.FromResult(model));

                // Act
                var result = await Controller.Put("13579", _validRequest);
                var body = ((OkObjectResult)result).Value as PublicViewDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.Name.Should().Be("My View");
            }

            [Fact]
            public async Task SetsUserIdWhenCreatingView()
            {
                // Arrange
                _mockPublicViewService.Setup(s =>
                        s.UpdatePublicView(It.IsAny<PublicView>(), _mockPublicViewRepository.Object, _mockMapper.Object))
                    .Returns((PublicView model, IDocumentRepository<Documents.PublicView> repo, IMapper mapper) => Task.FromResult(model));

                // Act
                await Controller.Put("13579", _validRequest);

                // Assert
                _mockPublicViewService.Verify(s => s.UpdatePublicView(It.Is<PublicView>(v => v.UserId == "12345"), _mockPublicViewRepository.Object, _mockMapper.Object));
            }
        }

        public class Delete : PublicViewManagementControllerTests
        {
            [Fact]
            public async Task ReturnsNotFoundWhenViewDoesNotExistForUser()
            {
                // Arrange
                _mockPublicViewService
                    .Setup(s => s.AssertUserOwnsPublicView("13579", "12345", _mockPublicViewRepository.Object))
                    .Throws<PublicViewNotFoundException>();

                // Act
                var result = await Controller.Delete("13579");

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                _mockPublicViewService.Verify(s => s.DeletePublicView(It.IsAny<string>(), _mockPublicViewRepository.Object), Times.Never);
            }

            [Fact]
            public async Task ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockPublicViewService.Setup(s => s.DeletePublicView("13579", _mockPublicViewRepository.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = await Controller.Delete("13579");

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task ReturnsOkWhenRequestIsSuccessful()
            {
                // Act
                var result = await Controller.Delete("13579");

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
