// <copyright file="BaseDocumentRepositoryTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using BackEnd.Infrastructure.Data;
    using BackEnd.Infrastructure.Exceptions;
    using FluentAssertions;
    using Microsoft.Azure.Cosmos;
    using Moq;
    using RPThreadTrackerV3.BackEnd.Interfaces.Data;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "BaseDocumentRepository")]
    public class BaseDocumentRepositoryTests
    {
        private readonly Mock<IDocumentClient<MockDocumentPoco>> _mockClient;
        private readonly BaseDocumentRepository<MockDocumentPoco> _repo;

        public BaseDocumentRepositoryTests()
        {
            _mockClient = new Mock<IDocumentClient<MockDocumentPoco>>();
            _repo = new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);
        }

        public class GetItemAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task ReturnsTypedObjectWhenRetrievalIsSuccessful()
            {
                // Arrange
                var document = new MockDocumentPoco();
                document.Name = "Test Name";
                document.Slug = "test-slug";
                document.Size = 15;
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>()))
                    .Returns(Task.FromResult(new List<MockDocumentPoco> { document }.AsEnumerable()));

                // Act
                var result = await _repo.GetItemAsync("documentid");

                // Assert
                result.Should().BeOfType<MockDocumentPoco>();
                result.Name.Should().Be("Test Name");
                result.Size.Should().Be(15);
                result.Slug.Should().Be("test-slug");
            }

            [Fact]
            public async Task ReturnsNullIfItemIsNotFound()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Exception("not found"), HttpStatusCode.NotFound);
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>()))
                    .Throws(exception);

                // Act
                var result = await _repo.GetItemAsync("documentid");

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public void RethrowsDocumentClientExceptionOtherThanNotFound()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Exception(), HttpStatusCode.BadRequest);
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>()))
                    .Throws(exception);

                // Act
                Func<Task> action = async () => await _repo.GetItemAsync("documentid");

                // Assert
                action.Should().Throw<AggregateException>()
                    .WithInnerException<DocumentDatabaseException>()
                        .WithInnerException<CosmosException>()
                        .Which.Should().Be(exception);
            }
        }

        public class GetItemsAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task ReturnsEmptyListIfNoResults()
            {
                // Arrange
                var mockResults = new List<MockDocumentPoco>();
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>())).Returns(Task.FromResult(mockResults.AsEnumerable()));

                // Act
                var result = await _repo.GetItemsAsync(p => p.Name == "Test");

                // Assert
                result.Should().HaveCount(0);
            }

            [Fact]
            public async Task ReturnsQueryResultsWhenQuerySuccessful()
            {
                // Arrange
                var mockResults = new List<MockDocumentPoco> { 
                    new MockDocumentPoco { Name = "Test 1" },
                    new MockDocumentPoco { Name = "Test 2" },
                    new MockDocumentPoco { Name = "Test 3" },
                    new MockDocumentPoco { Name = "Test 4" }
                };
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>())).Returns(Task.FromResult(mockResults.AsEnumerable()));

                // Act
                var result = await _repo.GetItemsAsync(p => p.Name == "Test");

                // Assert
                result.Should().HaveCount(4)
                    .And.Contain(p => p.Name == "Test 1")
                    .And.Contain(p => p.Name == "Test 2")
                    .And.Contain(p => p.Name == "Test 3")
                    .And.Contain(p => p.Name == "Test 4");
            }
        }

        public class CreateItemAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task ReturnsTypedObjectWhenCreationIsSuccessful()
            {
                // Arrange
                var item = new MockDocumentPoco
                {
                    Name = "Test Name",
                    Slug = "test-slug",
                    Size = 15
                };
                _mockClient.Setup(c => c.CreateDocumentAsync(item)).Returns(Task.FromResult(item));

                // Act
                var result = await _repo.CreateItemAsync(item);

                // Assert
                result.Should().BeOfType<MockDocumentPoco>();
                result.Name.Should().Be("Test Name");
                result.Size.Should().Be(15);
                result.Slug.Should().Be("test-slug");
            }
        }

        public class UpdateItemAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task ReturnsTypedObjectWhenCreationIsSuccessful()
            {
                // Arrange
                var item = new MockDocumentPoco
                {
                    Name = "Test Name",
                    Slug = "test-slug",
                    Size = 15
                };
                _mockClient.Setup(c => c.ReplaceDocumentAsync("12345", item)).Returns(Task.FromResult(item));

                // Act
                var result = await _repo.UpdateItemAsync("12345", item);

                // Assert
                result.Should().BeOfType<MockDocumentPoco>();
                result.Name.Should().Be("Test Name");
                result.Size.Should().Be(15);
                result.Slug.Should().Be("test-slug");
            }
        }

        public class DeleteItemAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task DeletesItemFromDatabase()
            {
                // Act
                await _repo.DeleteItemAsync("12345");

                // Assert
                _mockClient.Verify(c => c.DeleteDocumentAsync("12345"), Times.Once);
            }
        }
    }
}
