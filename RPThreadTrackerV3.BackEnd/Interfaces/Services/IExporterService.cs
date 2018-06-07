// <copyright file="IExporterService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using System.Collections.Generic;
    using Models.DomainModels;
    using OfficeOpenXml;

    /// <summary>
    /// Service for data manipulation relating to data exports.
    /// </summary>
    public interface IExporterService
    {
        /// <summary>
        /// Gets the byte array for an Excel file containing information about the given characters/threads.
        /// </summary>
        /// <param name="characters">The characters to be included in the file.</param>
        /// <param name="threads">The threads to be included in the file.</param>
        /// <returns>Array of bytes making up an Excel package to be downloaded by the user.</returns>
        ExcelPackage GetExcelPackage(IEnumerable<Character> characters, Dictionary<int, List<Thread>> threads);
    }
}
