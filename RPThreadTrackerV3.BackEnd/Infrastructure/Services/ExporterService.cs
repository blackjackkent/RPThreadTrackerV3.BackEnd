// <copyright file="ExporterService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using Interfaces.Services;
    using Models.DomainModels;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    /// <inheritdoc />
    public class ExporterService : IExporterService
    {
        /// <inheritdoc />
        public XSSFWorkbook GetExcelPackage(IEnumerable<Character> characters, Dictionary<int, List<Thread>> threads)
	    {
		    var workbook = new XSSFWorkbook();
            var headerStyle = GetHeaderStyle(workbook);
	        var bodyFieldStyle = GetBodyFieldStyle(workbook);
	        var archivedFieldStyle = GetArchivedFieldStyle(workbook);
		    foreach (var character in characters)
		    {
			    if (!threads.ContainsKey(character.CharacterId) || !threads[character.CharacterId].Any())
			    {
				    continue;
			    }
			    var worksheet = workbook.CreateSheet(character.UrlIdentifier);
		        var headerRow = worksheet.CreateRow(0);
		        GenerateCell(headerRow, 0, "Url Identifier", headerStyle);
		        GenerateCell(headerRow, 1, "Post ID", headerStyle);
		        GenerateCell(headerRow, 2, "User Title", headerStyle);
		        GenerateCell(headerRow, 3, "Partner Url Identifier", headerStyle);
		        GenerateCell(headerRow, 4, "Is Archived", headerStyle);

			    var i = 1;
				var threadsForCharacter = threads[character.CharacterId].OrderBy(t => t.IsArchived);
			    foreach (var thread in threadsForCharacter)
			    {
			        var row = worksheet.CreateRow(i);
			        var style = thread.IsArchived ? archivedFieldStyle : bodyFieldStyle;
                    GenerateCell(row, 0, character.UrlIdentifier, style);
                    GenerateCell(row, 1, thread.PostId, style);
			        GenerateCell(row, 2, thread.UserTitle, style);
                    GenerateCell(row, 3, thread.PartnerUrlIdentifier, style);
                    GenerateCell(row, 4, thread.IsArchived.ToString(CultureInfo.CurrentCulture), style);
				    i++;
				}

		        int lastColumNum = worksheet.GetRow(0).LastCellNum;
		        for (var col = 0; col <= lastColumNum; col++)
		        {
		            worksheet.AutoSizeColumn(col);
		        }
            }
	        return workbook;
	    }

        private static ICellStyle GetArchivedFieldStyle(XSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;
            var font = workbook.CreateFont();
            var color = new XSSFColor(new byte[] { 89, 89, 89 });
            font.Color = color.Indexed;
            style.SetFont(font);
            var format = workbook.CreateDataFormat().GetFormat("0");
            style.DataFormat = format;
            return style;
        }

        private static ICellStyle GetBodyFieldStyle(XSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            var format = workbook.CreateDataFormat().GetFormat("0");
            style.DataFormat = format;
            return style;
        }

        private static ICellStyle GetHeaderStyle(XSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            style.FillForegroundColor = IndexedColors.LightBlue.Index;
            style.FillPattern = FillPattern.SolidForeground;
            var font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.SetFont(font);
            return style;
        }

        private static void GenerateCell(IRow row, int index, string text, ICellStyle style)
        {
            var cell = row.CreateCell(index);
            cell.SetCellValue(text);
            cell.CellStyle = style;
        }
    }
}
