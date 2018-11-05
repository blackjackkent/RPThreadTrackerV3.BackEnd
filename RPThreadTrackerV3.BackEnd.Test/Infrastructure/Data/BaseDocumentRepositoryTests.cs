// <copyright file="BaseDocumentRepositoryTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using BackEnd.Infrastructure.Data;
    using BackEnd.Infrastructure.Exceptions;
    using FluentAssertions;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Moq;
    using TestHelpers;
    using Xunit;
    using IDocumentClient = Interfaces.Data.IDocumentClient;

    [Trait("Class", "BaseDocumentRepository")]
    public class BaseDocumentRepositoryTests
    {
        private readonly Mock<IDocumentClient> _mockClient;
        private readonly BaseDocumentRepository<MockDocumentPoco> _repo;

        public BaseDocumentRepositoryTests()
        {
            _mockClient = new Mock<IDocumentClient>();
            _repo = new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);
        }

        public class Constructor : BaseDocumentRepositoryTests
        {
            [Fact]
            public void CreatesDatabaseIfItDoesNotExist()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.NotFound);
                _mockClient.Setup(c => c.AssertDatabaseExists()).Throws(exception);

                // Act
                var repo = new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                _mockClient.Verify(c => c.CreateDatabaseAsync(), Times.Once);
            }

            [Fact]
            public void CreatesCollectionIfItDoesNotExist()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.NotFound);
                _mockClient.Setup(c => c.AssertCollectionExists()).Throws(exception);

                // Act
                var repo = new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                _mockClient.Verify(c => c.CreateDocumentCollectionAsync(), Times.Once);
            }

            [Fact]
            public void ThrowsIfUnexpectedDocumentClientExceptionOccursInReadingDatabase()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.BadGateway);
                _mockClient.Setup(c => c.AssertDatabaseExists()).Throws(exception);

                // Act
                Action action = () => new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                action.Should().Throw<AggregateException>()
                    .WithInnerException<DocumentDatabaseException>()
                        .WithInnerException<DocumentClientException>()
                        .Which.Should().Be(exception);
                _mockClient.Verify(c => c.CreateDatabaseAsync(), Times.Never);
            }

            [Fact]
            public void ThrowsIfUnexpectedDocumentClientExceptionOccursInReadingCollection()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.BadGateway);
                _mockClient.Setup(c => c.AssertCollectionExists()).Throws(exception);

                // Act
                Action action = () => new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                action.Should().Throw<AggregateException>()
                    .WithInnerException<DocumentDatabaseException>()
                        .WithInnerException<DocumentClientException>()
                        .Which.Should().Be(exception);
                _mockClient.Verify(c => c.CreateDocumentCollectionAsync(), Times.Never);
            }

            [Fact]
            public void ThrowsIfUnexpectedExceptionOccursInReadingDatabase()
            {
                // Arrange
                var exception = new NullReferenceException();
                _mockClient.Setup(c => c.AssertDatabaseExists()).Throws(exception);

                // Act
                Action action = () => new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                action.Should().Throw<AggregateException>()
                    .WithInnerException<DocumentDatabaseException>()
                        .WithInnerException<NullReferenceException>()
                        .Which.Should().Be(exception);
                _mockClient.Verify(c => c.CreateDatabaseAsync(), Times.Never);
            }

            [Fact]
            public void ThrowsIfUnexpectedExceptionOccursInReadingCollection()
            {
                // Arrange
                var exception = new NullReferenceException();
                _mockClient.Setup(c => c.AssertCollectionExists()).Throws(exception);

                // Act
                Action action = () => new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                action.Should().Throw<AggregateException>()
                    .WithInnerException<DocumentDatabaseException>()
                        .WithInnerException<NullReferenceException>()
                        .Which.Should().Be(exception);
                _mockClient.Verify(c => c.CreateDatabaseAsync(), Times.Never);
            }

            [Fact]
            public void DoesNothingIfDatabaseAndCollectionExist()
            {
                // Act
                var repo = new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object);

                // Assert
                _mockClient.Verify(c => c.CreateDatabaseAsync(), Times.Never);
                _mockClient.Verify(c => c.CreateDocumentCollectionAsync(), Times.Never);
            }
        }

        public class GetItemAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task ReturnsTypedObjectWhenRetrievalIsSuccessful()
            {
                // Arrange
                var document = new Document();
                document.SetPropertyValue("Name", "Test Name");
                document.SetPropertyValue("Slug", "test-slug");
                document.SetPropertyValue("Size", 15);
                _mockClient.Setup(c => c.ReadDocumentAsync("documentid")).Returns(Task.FromResult(document));

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
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.NotFound);
                _mockClient.Setup(c => c.ReadDocumentAsync("documentid")).Throws(exception);

                // Act
                var result = await _repo.GetItemAsync("documentid");

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public void RethrowsDocumentClientExceptionOtherThanNotFound()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.BadRequest);
                _mockClient.Setup(c => c.ReadDocumentAsync("documentid")).Throws(exception);

                // Act
                Func<Task> action = async () => await _repo.GetItemAsync("documentid");

                // Assert
                action.Should().Throw<AggregateException>()
                    .WithInnerException<DocumentDatabaseException>()
                        .WithInnerException<DocumentClientException>()
                        .Which.Should().Be(exception);
            }
        }

        public class GetItemsAsync : BaseDocumentRepositoryTests
        {
            [Fact]
            public async Task ReturnsEmptyListIfNoResults()
            {
                // Arrange
                var mockQuery = new Mock<IDocumentQuery<MockDocumentPoco>>();
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>())).Returns(mockQuery.Object);
                mockQuery.SetupGet(q => q.HasMoreResults).Returns(false);

                // Act
                var result = await _repo.GetItemsAsync(p => p.Name == "Test");

                // Assert
                result.Should().HaveCount(0);
            }

            [Fact]
            public async Task ReturnsQueryResultsWhenQuerySuccessful()
            {
                // Arrange
                var querySet1 = new FeedResponse<MockDocumentPoco>(new List<MockDocumentPoco>
                {
                    new MockDocumentPoco { Name = "Test 1" },
                    new MockDocumentPoco { Name = "Test 2" }
                });
                var querySet2 = new FeedResponse<MockDocumentPoco>(new List<MockDocumentPoco>
                {
                    new MockDocumentPoco { Name = "Test 3" },
                    new MockDocumentPoco { Name = "Test 4" }
                });
                var mockQuery = new Mock<IDocumentQuery<MockDocumentPoco>>();
                _mockClient.Setup(c => c.CreateDocumentQuery(It.IsAny<Expression<Func<MockDocumentPoco, bool>>>())).Returns(mockQuery.Object);
                mockQuery.SetupSequence(q => q.HasMoreResults)
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                mockQuery.SetupSequence(q => q.ExecuteNextAsync<MockDocumentPoco>(It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(querySet1))
                    .Returns(Task.FromResult(querySet2));

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
                var document = new Document();
                document.SetPropertyValue("Name", "Test Name");
                document.SetPropertyValue("Slug", "test-slug");
                document.SetPropertyValue("Size", 15);
                _mockClient.Setup(c => c.CreateDocumentAsync(item)).Returns(Task.FromResult(document));

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
                var document = new Document();
                document.SetPropertyValue("Name", "Test Name");
                document.SetPropertyValue("Slug", "test-slug");
                document.SetPropertyValue("Size", 15);
                _mockClient.Setup(c => c.ReplaceDocumentAsync("12345", item)).Returns(Task.FromResult(document));

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
