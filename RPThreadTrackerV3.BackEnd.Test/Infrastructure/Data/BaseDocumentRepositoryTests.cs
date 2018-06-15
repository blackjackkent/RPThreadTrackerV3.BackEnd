// <copyright file="BaseDocumentRepositoryTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Data
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using BackEnd.Infrastructure.Data;
    using BackEnd.Infrastructure.Exceptions;
    using FluentAssertions;
    using Microsoft.Azure.Documents;
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
    }
}
