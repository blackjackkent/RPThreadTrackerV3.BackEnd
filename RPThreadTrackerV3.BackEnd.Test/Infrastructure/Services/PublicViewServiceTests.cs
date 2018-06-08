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
                views.Should().HaveCount(2);
                views.Should().Contain(c => c.Id == "13579");
                views.Should().Contain(c => c.Id == "24680");
            }
        }

        public class CreatePublicView : PublicViewServiceTests
        {
            [Fact]
            public async Task ThrowsExceptionIfViewWithSlugAlreadyExists()
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

                // Act/Assert
                await Assert.ThrowsAsync<PublicViewSlugExistsException>(async () => await _publicViewService.CreatePublicView(newDocument, _mockPublicViewRepository.Object, _mockMapper.Object));
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
            public async Task ThrowsExceptionIfViewDoesNotExist()
            {
                // Arrange
                _mockPublicViewRepository.Setup(r => r.GetItemAsync("12345")).Returns(Task.FromResult((PublicView)null));

                // Act/Assert
                await Assert.ThrowsAsync<PublicViewNotFoundException>(async () => await _publicViewService.AssertUserOwnsPublicView("12345", "13579", _mockPublicViewRepository.Object));
            }

            [Fact]
            public async Task ThrowsExceptionIfViewDoesNotBelongToUser()
            {
                // Arrange
                var publicView = new PublicView
                {
                    UserId = "97531",
                    Id = "12345"
                };
                _mockPublicViewRepository.Setup(r => r.GetItemAsync("12345")).Returns(Task.FromResult(publicView));

                // Act/Assert
                await Assert.ThrowsAsync<PublicViewNotFoundException>(async () => await _publicViewService.AssertUserOwnsPublicView("12345", "13579", _mockPublicViewRepository.Object));
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
            public async Task ThrowsExceptionIfViewWithSlugAlreadyExistsOnAnotherView()
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

                // Act/Assert
                await Assert.ThrowsAsync<PublicViewSlugExistsException>(async () => await _publicViewService.UpdatePublicView(model, _mockPublicViewRepository.Object, _mockMapper.Object));
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
            public async Task ThrowsExceptionWhenViewDoesNotExist()
            {
                // Arrange
                _mockPublicViewRepository.Setup(r => r.GetItemsAsync(It.IsAny<Expression<Func<PublicView, bool>>>()))
                    .Returns(Task.FromResult(new List<PublicView>().AsEnumerable()));

                // Act/Assert
                await Assert.ThrowsAsync<PublicViewNotFoundException>(async () => await _publicViewService.GetViewBySlug("my-slug", _mockPublicViewRepository.Object, _mockMapper.Object));
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
    }
}
