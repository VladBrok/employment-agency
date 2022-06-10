using System.Text;

namespace EmploymentAgency.Reports;

public class HtmlReport : IReport
{
    public async Task BuildAsync(IEnumerable<Entity> entities, string outputFile, string title)
    {
        var report = new StringBuilder();
        WriteHeader(entities, report, title);
        WriteBody(entities, report);
        await WriteToFileAsync(outputFile, report);
    }

    private void WriteHeader(IEnumerable<Entity> entities, StringBuilder report, string title)
    {
        report
            .Append(
                @"
            <style>
              th {
                  text-transform: uppercase;
              }
              th, td {
                  border: 1px solid black;
                  padding: 6px;
                  text-align: center;
              }
              th:empty, td:empty {
                  display: none;
              }
              table {
                  border-collapse: collapse;
                  table-layout: fixed;
              }
              div {
                  display: flex;
                  flex-direction: column;
                  align-items: center;
              }
              .photo-container {
                  text-indent: -9999px;
                  margin-top: -1rem;
              }
              img {
                max-width: 90px
              }
            </style>"
            )
            .Append("<div><h1>")
            .Append(title)
            .Append("</h1><table><tr>");
        foreach (string columnName in entities.First().Keys)
        {
            report.Append("<th>").Append(columnName).Append("</th>");
        }
    }

    private void WriteBody(IEnumerable<Entity> entities, StringBuilder report)
    {
        foreach (var entity in entities)
        {
            report.Append("</tr><tr>");
            foreach (string data in entity.Values)
            {
                report.Append("<td>").Append(data).Append("</td>");
            }
        }
        report.Append("</tr></table></div>");
    }

    private async Task WriteToFileAsync(string file, StringBuilder report)
    {
        using var writer = new StreamWriter(file);
        await writer.WriteAsync(report);
    }
}
