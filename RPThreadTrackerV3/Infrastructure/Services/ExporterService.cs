namespace RPThreadTrackerV3.Infrastructure.Services
{
	using Interfaces.Services;
	using OfficeOpenXml;

	public class ExporterService : IExporterService
    {
	    public byte[] GetByteArray()
	    {
		    var package = new ExcelPackage();
		    var worksheet = package.Workbook.Worksheets.Add("Test Sheet");
		    worksheet.Cells["A1"].Value = "Test Content";
		    var bytes = package.GetAsByteArray();
		    return bytes;
	    }
    }
}
