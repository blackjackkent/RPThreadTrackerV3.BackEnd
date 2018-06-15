// <copyright file="ExporterServiceTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using BackEnd.Infrastructure.Services;
    using BackEnd.Models.DomainModels;
    using FluentAssertions;
    using Xunit;

    [Trait("Class", "ExporterService")]
    public class ExporterServiceTests
    {
        private readonly ExporterService _exporterService;

        public ExporterServiceTests()
        {
            _exporterService = new ExporterService();
        }

        public class GetExcelPackage : ExporterServiceTests
        {
            [Fact]
            public void ProducesEmptyWorkbookWhenNoCharactersProvided()
            {
                // Arrange
                var characters = new List<Character>();
                var threads = new Dictionary<int, List<Thread>>();

                // Act
                var result = _exporterService.GetExcelPackage(characters, threads);

                // Assert
                result.Workbook.Worksheets.Should().HaveCount(0);
            }

            [Fact]
            public void ProducesEmptyWorkbookWhenNoThreadsProvided()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character(),
                    new Character(),
                    new Character(),
                    new Character()
                };
                var threads = new Dictionary<int, List<Thread>>();

                // Act
                var result = _exporterService.GetExcelPackage(characters, threads);

                // Assert
                result.Workbook.Worksheets.Should().HaveCount(0);
            }

            [Fact]
            public void ProducesWorksheetForEachCharacterWithThreads()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character
                    {
                        CharacterId = 12345,
                        UrlIdentifier = "character1"
                    },
                    new Character
                    {
                        CharacterId = 23456,
                        UrlIdentifier = "character2"
                    },
                    new Character
                    {
                        CharacterId = 34567,
                        UrlIdentifier = "character3"
                    },
                    new Character
                    {
                        CharacterId = 45678,
                        UrlIdentifier = "character4"
                    }
                };
                var threads = new Dictionary<int, List<Thread>>
                {
                    { 12345, new List<Thread> { new Thread { IsArchived = false } } },
                    { 23456, new List<Thread>() },
                    { 34567, new List<Thread> { new Thread { IsArchived = true } } }
                };

                // Act
                var result = _exporterService.GetExcelPackage(characters, threads);

                // Assert
                result.Workbook.Worksheets.Should().HaveCount(2);
            }

            [Fact]
            public void ProducesWorksheetWithHeaderAndRowsForAllThreads()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character
                    {
                        CharacterId = 12345,
                        UrlIdentifier = "character1"
                    }
                };
                var threads = new Dictionary<int, List<Thread>>
                {
                    {
                        12345,
                        new List<Thread>
                        {
                            new Thread
                            {
                                ThreadId = 13579,
                                CharacterId = 12345,
                                PartnerUrlIdentifier = "my-partner",
                                PostId = "98765",
                                UserTitle = "My Title",
                                IsArchived = false
                            },
                            new Thread
                            {
                                ThreadId = 24680,
                                CharacterId = 12345,
                                PartnerUrlIdentifier = "my-other-partner",
                                PostId = "87654",
                                UserTitle = "My Title 2",
                                IsArchived = true
                            }
                        }
                    }
                };

                // Act
                var result = _exporterService.GetExcelPackage(characters, threads);
                var worksheet = result.Workbook.Worksheets.FirstOrDefault();

                // Assert
                worksheet.Cells["A1"].Text.Should().Be("Url Identifier");
                worksheet.Cells["B1"].Text.Should().Be("Post ID");
                worksheet.Cells["C1"].Text.Should().Be("User Title");
                worksheet.Cells["D1"].Text.Should().Be("Partner Url Identifier");
                worksheet.Cells["E1"].Text.Should().Be("Is Archived");

                worksheet.Cells["A2"].Text.Should().Be("character1");
                worksheet.Cells["B2"].Text.Should().Be("98765");
                worksheet.Cells["C2"].Text.Should().Be("My Title");
                worksheet.Cells["D2"].Text.Should().Be("my-partner");
                worksheet.Cells["E2"].Text.Should().Be("False");

                worksheet.Cells["A3"].Text.Should().Be("character1");
                worksheet.Cells["B3"].Text.Should().Be("87654");
                worksheet.Cells["C3"].Text.Should().Be("My Title 2");
                worksheet.Cells["D3"].Text.Should().Be("my-other-partner");
                worksheet.Cells["E3"].Text.Should().Be("True");
            }
        }
    }
}
