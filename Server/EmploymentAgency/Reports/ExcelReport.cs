using System.Drawing;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EmploymentAgency.Reports;

public class ExcelReport : IReport
{
    private readonly Regex _linkRegex =
        new("(http|https)://.+?[.](png|jpg|jpeg)", RegexOptions.Compiled);

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
        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Отчёт");
        sheet.Columns.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        sheet.DefaultRowHeight = 40;
        sheet.Row(1).Style.Font.Bold = true;
        sheet.Row(1).Style.Font.Size = 20;
        sheet.Row(2).Style.Font.Bold = true;
        return sheet;
    }

    private void WriteContent(IEnumerable<Entity> entities, string title, ExcelWorksheet sheet)
    {
        string[] columnNames = entities.First().Keys.Where(k => !string.IsNullOrEmpty(k)).ToArray();
        sheet.Cells["A1"].Value = title;
        sheet.Cells[$"A1:{(char)(64 + columnNames.Length)}1"].Merge = true;
        ExcelRangeBase range = sheet.Cells["A2"].LoadFromArrays(
            entities
                .Select((e, row) => ExtractValues(e, row + 3, sheet).ToArray())
                .Prepend(columnNames)
        );
        range.AutoFitColumns();
    }

    private IEnumerable<string> ExtractValues(Entity entity, int row, ExcelWorksheet sheet)
    {
        return entity.Values
            .Where(v => !string.IsNullOrEmpty(v))
            .Select(
                (value, column) =>
                {
                    string link = _linkRegex.Match(value).Value;
                    return string.IsNullOrEmpty(link)
                      ? value
                      : MakeLink(sheet.Cells[row, column + 1], link);
                }
            );
    }

    private string MakeLink(ExcelRange cell, string link)
    {
        cell.Formula = $"HYPERLINK(\"{link}\",\"{link}\")";
        cell.Style.Font.UnderLine = true;
        cell.Style.Font.Color.SetColor(Color.Blue);
        return link;
    }
}
