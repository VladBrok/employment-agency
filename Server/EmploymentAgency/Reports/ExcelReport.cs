using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EmploymentAgency.Reports;

public class ExcelReport : IReport
{
    public async Task BuildAsync(IEnumerable<Entity> entities, string outputFile, string title)
    {
        DeleteIfExists(outputFile);
        ExcelPackage package = MakePackage(outputFile);
        WriteContent(entities, title, MakeSheet(package));
        await package.SaveAsync();
    }

    private void DeleteIfExists(string file)
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }

    private ExcelPackage MakePackage(string outputFile)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        return new ExcelPackage(outputFile);
    }

    private ExcelWorksheet MakeSheet(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets.Add("Отчёт");

        sheet.Columns.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        sheet.Row(1).Style.Font.Bold = true;
        sheet.Row(1).Height = 25;
        sheet.Row(1).Style.Font.Size = 20;
        sheet.Row(2).Style.Font.Bold = true;

        return sheet;
    }

    private void WriteContent(IEnumerable<Entity> entities, string title, ExcelWorksheet sheet)
    {
        sheet.Cells["A1"].Value = title;
        sheet.Cells[$"A1:{(char)(64 + entities.First().Keys.Count)}1"].Merge = true;

        var range = sheet.Cells["A2"].LoadFromArrays(
            entities.Select(e => e.Values.ToArray()).Prepend(entities.First().Keys.ToArray())
        );
        range.AutoFitColumns();
    }
}
