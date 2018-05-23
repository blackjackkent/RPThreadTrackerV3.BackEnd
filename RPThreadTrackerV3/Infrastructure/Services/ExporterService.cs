// <copyright file="ExporterService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using Interfaces.Services;
	using Models.DomainModels;
	using OfficeOpenXml;

	public class ExporterService : IExporterService
    {
	    public byte[] GetByteArray(IEnumerable<Character> characters, Dictionary<int, List<Thread>> threads)
	    {
		    var package = new ExcelPackage();
		    foreach (var character in characters)
		    {
			    if (!threads.ContainsKey(character.CharacterId) || !threads[character.CharacterId].Any())
			    {
				    continue;
			    }
			    var worksheet = package.Workbook.Worksheets.Add(character.UrlIdentifier);
			    worksheet.Cells[1, 1].Value = "Url Identifier";
			    worksheet.Cells[1, 2].Value = "Post ID";
			    worksheet.Cells[1, 3].Value = "User Title";
			    worksheet.Cells[1, 4].Value = "Partner Url Identifier";
			    worksheet.Cells[1, 5].Value = "Is Archived";

			    var i = 2;
				var threadsForCharacter = threads[character.CharacterId].OrderBy(t => t.IsArchived);
			    foreach (var thread in threadsForCharacter)
			    {
					worksheet.Cells["A" + i].Value = character.UrlIdentifier;
				    worksheet.Cells["B" + i].Value = thread.PostId;
				    worksheet.Cells["C" + i].Value = thread.UserTitle;
				    worksheet.Cells["D" + i].Value = thread.PartnerUrlIdentifier;
				    worksheet.Cells["E" + i].Value = thread.IsArchived;
				    if (thread.IsArchived)
				    {
						worksheet.Cells["A" + i + ":E" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
					    worksheet.Cells["A" + i + ":E" + i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
					    worksheet.Cells["A" + i + ":E" + i].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#595959"));
					}
				    i++;
				}
			    worksheet.Cells["A1:E1"].Style.Font.Bold = true;
			    worksheet.Cells["A1:E1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
			    worksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
			    worksheet.Cells["A1:E" + i].AutoFitColumns();
			    worksheet.Cells["B2:B" + i].Style.Numberformat.Format = "0";
			}
		    var bytes = package.GetAsByteArray();
		    return bytes;
	    }
    }
}
