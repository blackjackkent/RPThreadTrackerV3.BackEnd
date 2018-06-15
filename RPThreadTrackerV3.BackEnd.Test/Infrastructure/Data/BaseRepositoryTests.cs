// <copyright file="BaseRepositoryTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using BackEnd.Infrastructure.Data;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using TestHelpers;
    using TestHelpers.Data;
    using Xunit;

    [Trait("Class", "BaseRepository")]
    public class BaseRepositoryTests
    {
        private readonly BaseRepository<MockEntityPoco> _repo;
        private readonly Mock<MockTrackerContext> _mockContext;
        private Mock<DbSet<MockEntityPoco>> _mockSet;
        private Mock<DbSet<MockEntityNavigationProperty>> _mockNavigationSet;

        public BaseRepositoryTests()
        {
            _mockContext = new Mock<MockTrackerContext>();
            SetupPocoSet();
            SetupNavigationSet();
            _repo = new BaseRepository<MockEntityPoco>(_mockContext.Object);
        }

        private void SetupNavigationSet()
        {
            var records = new List<MockEntityNavigationProperty>
            {
                new MockEntityNavigationProperty { MockEntityNavigationPropertyId = "1" },
                new MockEntityNavigationProperty { MockEntityNavigationPropertyId = "2" },
                new MockEntityNavigationProperty { MockEntityNavigationPropertyId = "3" }
            };
            var queryableRecords = records.AsQueryable();
            _mockNavigationSet = new Mock<DbSet<MockEntityNavigationProperty>>();
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.Provider).Returns(queryableRecords.Provider);
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.Expression).Returns(queryableRecords.Expression);
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.ElementType).Returns(queryableRecords.ElementType);
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.GetEnumerator()).Returns(queryableRecords.GetEnumerator());
            _mockContext.SetupGet(c => c.MockEntityNavigationProperties).Returns(_mockNavigationSet.Object);
            _mockContext.Setup(c => c.Set<MockEntityNavigationProperty>()).Returns(_mockNavigationSet.Object);
        }

        private void SetupPocoSet()
        {
            var nav1 = new MockEntityNavigationProperty { MockEntityNavigationPropertyId = "1" };
            var nav2 = new MockEntityNavigationProperty { MockEntityNavigationPropertyId = "2" };
            var nav3 = new MockEntityNavigationProperty { MockEntityNavigationPropertyId = "3" };
            var records = new List<MockEntityPoco>
            {
                new MockEntityPoco { Name = "Name 1", MockEntityNavigationPropertyId = "1", MockEntityNavigationProperty = nav1 },
                new MockEntityPoco { Name = "Name 2", MockEntityNavigationPropertyId = "2", MockEntityNavigationProperty = nav2 },
                new MockEntityPoco { Name = "Name 3", MockEntityNavigationPropertyId = "3", MockEntityNavigationProperty = nav3 },
            };
            var navRecords = new List<MockEntityNavigationProperty> { nav1, nav2, nav3 };

            var queryableRecords = records.AsQueryable();
            _mockSet = new Mock<DbSet<MockEntityPoco>>();
            _mockSet.As<IQueryable<MockEntityPoco>>().Setup(m => m.Provider).Returns(queryableRecords.Provider);
            _mockSet.As<IQueryable<MockEntityPoco>>().Setup(m => m.Expression).Returns(queryableRecords.Expression);
            _mockSet.As<IQueryable<MockEntityPoco>>().Setup(m => m.ElementType).Returns(queryableRecords.ElementType);
            _mockSet.As<IQueryable<MockEntityPoco>>().Setup(m => m.GetEnumerator()).Returns(queryableRecords.GetEnumerator());
            _mockContext.SetupGet(c => c.MockEntityPocos).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.Set<MockEntityPoco>()).Returns(_mockSet.Object);

            var queryableNavRecords = navRecords.AsQueryable();
            _mockNavigationSet = new Mock<DbSet<MockEntityNavigationProperty>>();
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.Provider).Returns(queryableNavRecords.Provider);
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.Expression).Returns(queryableNavRecords.Expression);
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.ElementType).Returns(queryableNavRecords.ElementType);
            _mockNavigationSet.As<IQueryable<MockEntityNavigationProperty>>().Setup(m => m.GetEnumerator()).Returns(queryableNavRecords.GetEnumerator());
            _mockContext.SetupGet(c => c.MockEntityNavigationProperties).Returns(_mockNavigationSet.Object);
            _mockContext.Setup(c => c.Set<MockEntityNavigationProperty>()).Returns(_mockNavigationSet.Object);
        }

        public class Create : BaseRepositoryTests
        {
            [Fact]
            public void CreatesRecordAndReturnsEntity()
            {
                // Arrange
                var entity = new MockEntityPoco
                {
                    Name = "My Entity",
                    Slug = "my-slug",
                    Size = 15
                };

                // Act
                var result = _repo.Create(entity);

                // Assert
                result.Should().Be(entity);
                _mockContext.Verify(c => c.Add(entity), Times.Once);
                _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            }
        }

        public class Delete : BaseRepositoryTests
        {
            [Fact]
            public void ReturnsTrueWhenRowDeleted()
            {
                // Arrange
                _mockContext.Setup(c => c.SaveChanges()).Returns(1);

                // Act
                var result = _repo.Delete(new MockEntityPoco());

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ReturnsTrueWhenNoRowsAffected()
            {
                // Arrange
                _mockContext.Setup(c => c.SaveChanges()).Returns(0);

                // Act
                var result = _repo.Delete(new MockEntityPoco());

                // Assert
                result.Should().BeFalse();
            }
        }

        public class ExistsWhere : BaseRepositoryTests
        {
            [Fact]
            public void ReturnsTrueWhenMatchingRecordsExist()
            {
                // Act
                var result = _repo.ExistsWhere(p => p.Name == "Name 2");

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ReturnsFalseWhenNoMatchingRecordsExist()
            {
                // Act
                var result = _repo.ExistsWhere(p => p.Name == "Name 4");

                // Assert
                result.Should().BeFalse();
            }
        }

        public class GetAll : BaseRepositoryTests
        {
            [Fact]
            public void ReturnsQueryResultsIfNoPropertiesProvided()
            {
                // Arrange

                // Act
                var result = _repo.GetAll();

                // Assert
                result.Should().HaveCount(3);
            }
        }
    }
}
