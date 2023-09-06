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
                .Returns((Delegate)((PublicView entity) => new DomainModels.PublicView
                {
                    UserId = entity.UserId,
                    id = entity.id,
                    Slug = entity.Slug,
                    Name = entity.Name,
                    TurnFilter = new DomainModels.PublicTurnFilter
                    {
                        IncludeMyTurn = entity.TurnFilter?.IncludeMyTurn ?? false,
                        IncludeArchived = entity.TurnFilter?.IncludeArchived ?? false,
                        IncludeQueued = entity.TurnFilter?.IncludeQueued ?? false,
                        IncludeTheirTurn = entity.TurnFilter?.IncludeTheirTurn ?? false
                    },
                    Columns = entity.Columns,
                    CharacterIds = entity.CharacterIds,
                    SortKey = entity.SortKey,
                    Tags = entity.Tags,
                    SortDescending = entity.SortDescending
                }));
            _mockMapper.Setup(m => m.Map<PublicView>(It.IsAny<DomainModels.PublicView>()))
                .Returns((DomainModels.PublicView model) => new PublicView
                {
                    UserId = model.UserId,
                    id = model.id,
                    Slug = model.Slug,
                    Name = model.Name,
                    TurnFilter = new PublicTurnFilter
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
                    id = "13579",
                    Name = "View 1",
                    Slug = "view-1",
                    CharacterIds = new List<int> { 13579 },
                    Columns = new List<string> { "column1" },
                    SortDescending = false,
                    SortKey = "column1",
                    Tags = new List<string> { "tag 1", "tag 2" },
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeMyTurn = true,
                        IncludeTheirTurn = false,
                        IncludeArchived = true,
                        IncludeQueued = false
                    }
                };
                var view2 = new PublicView
                {
                    UserId = "12345",
                    id = "24680",
                    Name = "View 2",
                    Slug = "view-2",
                    CharacterIds = new List<int> { 13579 },
                    Columns = new List<string> { "column1" },
                    SortDescending = false,
                    SortKey = "column1",
                    Tags = new List<string> { "tag 1", "tag 2" },
                    TurnFilter = new PublicTurnFilter
                    {
                        IncludeMyTurn = true,
                        IncludeTheirTurn = false,
                        IncludeArchived = true,
                        IncludeQueued = false
                    }
                };
                var viewList = new List<PublicView> { view1, view2 };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(view1) && y.Compile()(view2)))).Returns(Task.FromResult(viewList.AsEnumerable()));

                // Act
                var views = await _publicViewService.GetPublicViews("12345", _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                views.Should().HaveCount(2)
                    .And.Contain(c => c.id == "13579")
                    .And.Contain(c => c.id == "24680");
            }
        }

        public class CreatePublicView : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfViewWithSlugAlreadyExistsForUser()
            {
                // Arrange
                var existingDocument = new PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-slug"
                };
                var newDocument = new DomainModels.PublicView
                {
                    id = "23456",
                    UserId = "12345",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingDocument)))).Returns(Task.FromResult(new List<PublicView> { existingDocument }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.CreatePublicView(newDocument, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<InvalidPublicViewSlugException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewWithSlugAlreadyExistsForAnotherUser()
            {
                // Arrange
                var existingDocument = new PublicView
                {
                    id = "13579",
                    Slug = "my-slug"
                };
                var newDocument = new DomainModels.PublicView
                {
                    id = "23456",
                    UserId = "54321",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingDocument)))).Returns(Task.FromResult(new List<PublicView> { existingDocument }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.CreateItemAsync(It.IsAny<PublicView>())).Returns((PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.CreatePublicView(newDocument, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.CreateItemAsync(It.Is<PublicView>(v => v.id == "23456" && v.Slug == "my-slug")));
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewWithSlugDoesNotExist()
            {
                // Arrange
                var newDocument = new DomainModels.PublicView
                {
                    id = "23456",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.IsAny<Expression<Func<PublicView, bool>>>())).Returns(Task.FromResult(new List<PublicView>().AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.CreateItemAsync(It.IsAny<PublicView>())).Returns((PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.CreatePublicView(newDocument, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.CreateItemAsync(It.Is<PublicView>(v => v.id == "23456" && v.Slug == "my-slug")));
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
                    id = "12345"
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
                    id = "12345"
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
            public void ThrowsExceptionIfViewWithSlugAlreadyExistsOnAnotherViewForUser()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-slug"
                };
                var existingView = new PublicView
                {
                    id = "98765",
                    UserId = "12345",
                    Slug = "my-slug"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<InvalidPublicViewSlugException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewWithSlugAlreadyExistsOnAnotherViewForAnotherUser()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-slug",
                    Name = "New View Name"
                };
                var existingView = new PublicView
                {
                    id = "97531",
                    UserId = "54321",
                    Slug = "my-slug",
                    Name = "Old View Name"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.UpdateItemAsync("13579", It.IsAny<PublicView>())).Returns((string id, PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.UpdateItemAsync("13579", It.Is<PublicView>(v => v.id == "13579" && v.Slug == "my-slug" && v.Name == "New View Name")), Times.Once);
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewIsNotChangingSlug()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-slug",
                    Name = "New View Name"
                };
                var existingView = new PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-slug",
                    Name = "Old View Name"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.UpdateItemAsync("13579", It.IsAny<PublicView>())).Returns((string id, PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.UpdateItemAsync("13579", It.Is<PublicView>(v => v.id == "13579" && v.Slug == "my-slug" && v.Name == "New View Name")), Times.Once);
            }

            [Fact]
            public async Task ThrowsNoExceptionIfViewIsChangingSlug()
            {
                // Arrange
                var model = new DomainModels.PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-slug",
                    Name = "New View Name"
                };
                var existingView = new PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-old-slug",
                    Name = "Old View Name"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView)))).Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.UpdateItemAsync("13579", It.IsAny<PublicView>())).Returns((string id, PublicView entity) => Task.FromResult(entity));

                // Act
                await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                _mockPublicViewRepository.Verify(r => r.UpdateItemAsync("13579", It.Is<PublicView>(v => v.id == "13579" && v.Slug == "my-slug" && v.Name == "New View Name")), Times.Once);
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

        public class GetViewBySlugAndUserId : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionWhenViewDoesNotExist()
            {
                // Arrange
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.IsAny<Expression<Func<PublicView, bool>>>()))
                    .Returns(Task.FromResult(new List<PublicView>().AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.GetViewBySlugAndUserId("my-slug", "12345", _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<PublicViewNotFoundException>();
            }

            [Fact]
            public void ThrowsExceptionWhenViewExistsNotForUser()
            {
                // Arrange
                var view = new PublicView
                {
                    UserId = "54321",
                    Slug = "my-slug",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(view))))
                    .Returns(Task.FromResult(new List<PublicView> { view }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.GetViewBySlugAndUserId("my-slug", "12345", _mockPublicViewRepository.Object, _mockMapper.Object);

                // Assert
                action.Should().Throw<PublicViewNotFoundException>();
            }

            [Fact]
            public async Task ThrowsNoExceptionWhenViewExistsForUser()
            {
                // Arrange
                var view = new PublicView
                {
                    UserId = "12345",
                    Slug = "my-slug",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(view))))
                    .Returns(Task.FromResult(new List<PublicView> { view }.AsEnumerable()));

                // Act
                var result = await _publicViewService.GetViewBySlugAndUserId("my-slug", "12345", _mockPublicViewRepository.Object, _mockMapper.Object);

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
                result.id.Should().BeNullOrEmpty();
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

        public class AssertSlugIsValid : PublicViewServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfSlugContainsInvalidCharacters()
            {
                // Act
                Func<Task> action = async () => await _publicViewService.AssertSlugIsValid("my turn", "13579", "12345", _mockPublicViewRepository.Object);

                // Assert
                action.Should().Throw<InvalidPublicViewSlugException>();
            }

            [Fact]
            public void ThrowsExceptionIfSlugExistsForUserAndViewIdIsNull()
            {
                // Arrange
                var existingView = new PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-view",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView))))
                    .Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertSlugIsValid("my-view", null, "12345", _mockPublicViewRepository.Object);

                // Assert
                action.Should().Throw<InvalidPublicViewSlugException>();
            }

            [Fact]
            public void ThrowsExceptionIfSlugExistsForUserAndViewIdDoesNotMatch()
            {
                // Arrange
                var existingView = new PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-view",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView))))
                    .Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertSlugIsValid("my-view", "23456", "12345", _mockPublicViewRepository.Object);

                // Assert
                action.Should().Throw<InvalidPublicViewSlugException>();
            }

            [Fact]
            public void ThrowsNoExceptionIfSlugExistsForUserAndViewIdMatches()
            {
                // Arrange
                var existingView = new PublicView
                {
                    id = "13579",
                    UserId = "12345",
                    Slug = "my-view",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView))))
                    .Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertSlugIsValid("my-view", "13579", "12345", _mockPublicViewRepository.Object);

                // Assert
                action.Should().NotThrow<InvalidPublicViewSlugException>();
            }

            [Fact]
            public void ThrowsNoExceptionIfSlugExistsForAnotherUser()
            {
                // Arrange
                var existingView = new PublicView
                {
                    id = "98765",
                    UserId = "54321",
                    Slug = "my-view",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView))))
                    .Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertSlugIsValid("my-view", "13579", "12345", _mockPublicViewRepository.Object);

                // Assert
                action.Should().NotThrow<InvalidPublicViewSlugException>();
            }

            [Fact]
            public void ThrowsNoExceptionIfSlugDoesNotExist()
            {
                // Arrange
                var existingView = new PublicView
                {
                    id = "13579",
                    Slug = "my-view",
                    Name = "My View"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.Is<Expression<Func<PublicView, bool>>>(y => y.Compile()(existingView))))
                    .Returns(Task.FromResult(new List<PublicView> { existingView }.AsEnumerable()));
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.IsAny<Expression<Func<PublicView, bool>>>()))
                    .Returns(Task.FromResult(new List<PublicView>().AsEnumerable()));

                // Act
                Func<Task> action = async () => await _publicViewService.AssertSlugIsValid("my-view2", "13579", "12345", _mockPublicViewRepository.Object);

                // Assert
                action.Should().NotThrow<InvalidPublicViewSlugException>();
            }
        }
    }
}
