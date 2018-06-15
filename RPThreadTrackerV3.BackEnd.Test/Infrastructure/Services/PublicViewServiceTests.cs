// <copyright file="PublicViewServiceTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using BackEnd.Infrastructure.Data.Documents;
    using BackEnd.Infrastructure.Exceptions.PublicViews;
    using BackEnd.Infrastructure.Services;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.ViewModels.PublicViews;
    using FluentAssertions;
    using Interfaces.Data;
    using Moq;
    using Xunit;
    using DomainModels = BackEnd.Models.DomainModels.PublicViews;

    [Trait("Class", "PublicViewService")]
    public class PublicViewServiceTests
    {
        private readonly PublicViewService _publicViewService;
        private readonly Mock<IDocumentRepository<PublicView>> _mockPublicViewRepository;
        private readonly Mock<IMapper> _mockMapper;

        public PublicViewServiceTests()
        {
            _mockPublicViewRepository = new Mock<IDocumentRepository<PublicView>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<DomainModels.PublicView>(It.IsAny<PublicView>()))
                .Returns((PublicView entity) => new DomainModels.PublicView
                {
                    UserId = entity.UserId,
                    Id = entity.Id,
                    Slug = entity.Slug,
                    Name = entity.Name
                });
            _mockMapper.Setup(m => m.Map<PublicView>(It.IsAny<DomainModels.PublicView>()))
                .Returns((DomainModels.PublicView model) => new PublicView
                {
                    UserId = model.UserId,
                    Id = model.Id,
                    Slug = model.Slug,
                    Name = model.Name
                });
            _publicViewService = new PublicViewService();
        }

        public class GetPublicViews : PublicViewServiceTests
        {
            [Fact]
            public async Task GetsAllPublicViewsForUser()
            {
                // Arrange
                var view1 = new PublicView
                {
                    UserId = "12345",
                    Id = "13579"
                };
                var view2 = new PublicView
                {
                    UserId = "12345",
                    Id = "24680"
                };
                var viewList = new List<PublicView> { view1, view2 };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(view1) && y.Compile()(view2)))).Returns(Task.FromResult(viewList.AsEnumerable()));

                // Act
                var views = await _publicViewService.GetPublicViews("12345", _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                views.Should().HaveCount(2)
                    .And.Contain(c => c.Id == "13579")
                    .And.Contain(c => c.Id == "24680");
            }
        }

        public class CreatePublicView : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfViewWithSlugAlreadyExists()
            {
                // Arrange
                var existingDocument = new PublicView
                {
                    Id = "12345",
                    Slug = "my-slug"
                };
                var newDocument = new DomainModels.PublicView
                {
                    Id = "23456",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingDocument)))).Returns(Task.FromResult(new List<PublicView> { existingDocument }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.CreatePublicView(newDocument, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<PublicViewSlugExistsException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewCanBeCreated()
            {
                // Arrange
                var newDocument = new DomainModels.PublicView
                {
                    Id = "23456",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.IsAny<Expression<Func<PublicView, bool>>>())).Returns(Task.FromResult(new List<PublicView>().AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.CreateItemAsync(It.IsAny<PublicView>())).Returns((PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.CreatePublicView(newDocument, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.CreateItemAsync(It.Is<PublicView>(v => v.Id == "23456" && v.Slug == "my-slug")));
            }
        }

        public class AssertUserOwnsPublicView : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfViewDoesNotExist()
            {
                // Arrange
                _mockPublicViewRepository.Setup(r => r.GetItemAsync("12345")).Returns(Task.FromResult((PublicView)null));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertUserOwnsPublicView("12345", "13579", _mockPublicViewRepository.Object);

                // Assert
                action.Should().Throw<PublicViewNotFoundException>();
            }

            [Fact]
            public void ThrowsExceptionIfViewDoesNotBelongToUser()
            {
                // Arrange
                var publicView = new PublicView
                {
                    UserId = "97531",
                    Id = "12345"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemAsync("12345")).Returns(Task.FromResult(publicView));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertUserOwnsPublicView("12345", "13579", _mockPublicViewRepository.Object);

                // Assert
                action.Should().Throw<PublicViewNotFoundException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewExistsAndBelongsToUser()
            {
                // Arrange
                var publicView = new PublicView
                {
                    UserId = "13579",
                    Id = "12345"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemAsync("12345")).Returns(Task.FromResult(publicView));

                // Act
                await _publicViewService.AssertUserOwnsPublicView("12345", "13579", _mockPublicViewRepository.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.GetItemAsync("12345"), Times.Once);
            }
        }

        public class UpdatePublicView : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfViewWithSlugAlreadyExistsOnAnotherView()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    Id = "12345",
                    Slug = "my-slug"
                };
                var existingView = new PublicView
                {
                    Id = "98765",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<PublicViewSlugExistsException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewIsNotChangingSlug()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    Id = "12345",
                    Slug = "my-slug",
                    Name = "New View Name"
                };
                var existingView = new PublicView
                {
                    Id = "12345",
                    Slug = "my-slug",
                    Name = "Old View Name"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.UpdateItemAsync("12345", It.IsAny<PublicView>())).Returns((string id, PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.UpdateItemAsync("12345", It.Is<PublicView>(v => v.Id == "12345" && v.Slug == "my-slug" && v.Name == "New View Name")), Times.Once);
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewIsChangingSlug()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    Id = "12345",
                    Slug = "my-slug",
                    Name = "New View Name"
                };
                var existingView = new PublicView
                {
                    Id = "12345",
                    Slug = "my-old-slug",
                    Name = "Old View Name"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.UpdateItemAsync("12345", It.IsAny<PublicView>())).Returns((string id, PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.UpdateItemAsync("12345", It.Is<PublicView>(v => v.Id == "12345" && v.Slug == "my-slug" && v.Name == "New View Name")), Times.Once);
            }
        }

        public class DeletePublicView : PublicViewServiceTests
        {
            [Fact]
            public async Task DeletesPublicView()
            {
                // Act
                await _publicViewService.DeletePublicView("12345", _mockPublicViewRepository.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.DeleteItemAsync("12345"), Times.Once);
            }
        }

        public class GetViewBySlug : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionWhenViewDoesNotExist()
            {
                // Arrange
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.IsAny<Expression<Func<PublicView, bool>>>()))
                    .Returns(Task.FromResult(new List<PublicView>().AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.GetViewBySlug("my-slug", _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<PublicViewNotFoundException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionWhenViewExist()
            {
                // Arrange
                var view = new PublicView
                {
                    Slug = "my-slug",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(view))))
                    .Returns(Task.FromResult(new List<PublicView> { view }.AsEnumerable()));

                // Act
                var result = await _publicViewService.GetViewBySlug("my-slug", _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                result.Slug.Should().Be("my-slug");
                result.Name.Should().Be("My View");
            }
        }

        public class GetViewFromLegacyDto : PublicViewServiceTests
        {
            private readonly LegacyPublicViewDto _legacyDto;

            public GetViewFromLegacyDto()
            {
                _legacyDto = new LegacyPublicViewDto
                {
                    UserId = "12345",
                    Slug = "my-view",
                    Name = "My View",
                    SortDescending = true,
                    CharacterUrlIdentifier = "my-character",
                    Columns = new List<string> { "column1", "column2" },
                    SortKey = "column2",
                    Tags = new List<string> { "tag1", "tag2", "tag3" },
                    TurnFilter = new PublicTurnFilterDto
                    {
                        IncludeMyTurn = true,
                        IncludeTheirTurn = true,
                        IncludeArchived = true,
                        IncludeQueued = true
                    }
                };
            }

            [Fact]
            public void PopulatesSimpleFields()
            {
                // Act
                var result = _publicViewService.GetViewFromLegacyDto(_legacyDto, new List<Character>());

                // Assert
                result.Id.Should().BeNullOrEmpty();
                result.Name.Should().Be("My View");
                result.Slug.Should().Be("my-view");
                result.CharacterIds.Should().HaveCount(0);
                result.Columns.Should().HaveCount(2);
                result.SortKey.Should().Be("column2");
                result.SortDescending.Should().BeTrue();
                result.Tags.Should().HaveCount(3);
                result.TurnFilter.IncludeMyTurn.Should().BeTrue();
                result.TurnFilter.IncludeTheirTurn.Should().BeTrue();
                result.TurnFilter.IncludeArchived.Should().BeTrue();
                result.TurnFilter.IncludeQueued.Should().BeTrue();
                result.UserId.Should().Be("12345");
            }

            [Fact]
            public void PopulatesCharacterFromUrlIdentifier()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character
                    {
                        UrlIdentifier = "my-character",
                        CharacterId = 13579
                    },
                    new Character
                    {
                        UrlIdentifier = "my-other-character",
                        CharacterId = 97531
                    }
                };

                // Act
                var result = _publicViewService.GetViewFromLegacyDto(_legacyDto, characters);

                // Assert
                result.CharacterIds.Should().HaveCount(1);
                result.CharacterIds.Should().Contain(13579);
            }

            [Fact]
            public void PopulatesAllCharactersIfNoUrlIdentifierProvided()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character
                    {
                        UrlIdentifier = "my-character",
                        CharacterId = 13579
                    },
                    new Character
                    {
                        UrlIdentifier = "my-other-character",
                        CharacterId = 97531
                    }
                };
                _legacyDto.CharacterUrlIdentifier = null;

                // Act
                var result = _publicViewService.GetViewFromLegacyDto(_legacyDto, characters);

                // Assert
                result.CharacterIds.Should().HaveCount(2);
                result.CharacterIds.Should().Contain(13579);
                result.CharacterIds.Should().Contain(97531);
            }
        }
    }
}
