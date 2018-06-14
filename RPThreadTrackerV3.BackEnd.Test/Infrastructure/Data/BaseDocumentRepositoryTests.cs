// <copyright file="BaseDocumentRepositoryTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Data
{
    using System;
    using System.Linq;
    using System.Net;
    using BackEnd.Infrastructure.Data;
    using BackEnd.Infrastructure.Exceptions;
    using FluentAssertions;
    using Interfaces.Data;
    using Microsoft.Azure.Documents;
    using Moq;
    using TestHelpers;
    using Xunit;

    [Trait("Class", "BaseDocumentRepository")]
    public class BaseDocumentRepositoryTests
    {
        private readonly Mock<IDocumentClient<MockDocumentPoco>> _mockClient;

        public BaseDocumentRepositoryTests()
        {
            _mockClient = new Mock<IDocumentClient<MockDocumentPoco>>();
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
            public void ThrowsIfUnexpectedErrorOccursInReadingDatabase()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.BadGateway);
                _mockClient.Setup(c => c.AssertDatabaseExists()).Throws(exception);

                // Act/Assert
                var thrown = Assert.Throws<AggregateException>(() => new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object));
                thrown.InnerExceptions.Count.Should().Be(1);
                thrown.InnerExceptions.FirstOrDefault().Should().BeOfType<DocumentDatabaseInitializationException>();
                thrown.InnerExceptions.FirstOrDefault().InnerException.Should().Be(exception);
                _mockClient.Verify(c => c.CreateDatabaseAsync(), Times.Never);
            }

            [Fact]
            public void ThrowsIfUnexpectedErrorOccursInReadingCollection()
            {
                // Arrange
                var exception = ExceptionBuilder.BuildDocumentClientException(new Error(), HttpStatusCode.BadGateway);
                _mockClient.Setup(c => c.AssertCollectionExists()).Throws(exception);

                // Act/Assert
                var thrown = Assert.Throws<AggregateException>(() => new BaseDocumentRepository<MockDocumentPoco>(_mockClient.Object));
                thrown.InnerExceptions.Count.Should().Be(1);
                thrown.InnerExceptions.FirstOrDefault().Should().BeOfType<DocumentDatabaseInitializationException>();
                thrown.InnerExceptions.FirstOrDefault().InnerException.Should().Be(exception);
                _mockClient.Verify(c => c.CreateDocumentCollectionAsync(), Times.Never);
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
    }
}
