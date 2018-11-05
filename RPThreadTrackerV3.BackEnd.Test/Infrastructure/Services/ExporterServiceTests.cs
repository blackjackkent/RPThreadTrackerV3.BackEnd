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
    using NPOI.SS.Formula.Functions;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
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
                var threads = new Dictionary<int, List<Thread>>();

                // Act
                var result = _exporterService.GetExcelPackage(new List<Character>(), threads);

                // Assert
                result.NumberOfSheets.Should().Be(0);
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
                result.NumberOfSheets.Should().Be(0);
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
                result.NumberOfSheets.Should().Be(2);
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
                var worksheet = result.GetSheetAt(0);

                // Assert
                AssertCellIsValid(worksheet, "A1", "Url Identifier");
                AssertCellIsValid(worksheet, "B1", "Post ID");
                AssertCellIsValid(worksheet, "C1", "User Title");
                AssertCellIsValid(worksheet, "D1", "Partner Url Identifier");
                AssertCellIsValid(worksheet, "E1", "Is Archived");

                AssertCellIsValid(worksheet, "A2", "character1");
                AssertCellIsValid(worksheet, "B2", "98765");
                AssertCellIsValid(worksheet, "C2", "My Title");
                AssertCellIsValid(worksheet, "D2", "my-partner");
                AssertCellIsValid(worksheet, "E2", "False");

                AssertCellIsValid(worksheet, "A3", "character1");
                AssertCellIsValid(worksheet, "B3", "87654");
                AssertCellIsValid(worksheet, "C3", "My Title 2");
                AssertCellIsValid(worksheet, "D3", "my-other-partner");
                AssertCellIsValid(worksheet, "E3", "True");
            }

            private static void AssertCellIsValid(ISheet worksheet, string reference, string value)
            {
                var cellReference = new CellReference(reference);
                var row = worksheet.GetRow(cellReference.Row);
                var cell = row.GetCell(cellReference.Col);
                cell.StringCellValue.Should().Be(value);
            }
        }
    }
}
