using EmploymentAgency.Reports;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentAgency.EndpointMappers;

public static class ReportsMapper
{
    public static void Map(WebApplication app)
    {
        app.MapPost(
            "api/reports/{{type}}",
            async (
                string fileName,
                string type,
                string title,
                [FromBody] IEnumerable<Entity> entities,
                HttpResponse response
            ) =>
            {
                var report = new ReportFactory().MakeReport(type);
                await report.BuildAsync(entities, fileName, title);

                response.Headers.Add("content-disposition", $"attachment;filename={fileName}");
                response.ContentType = "application/octet-stream";
                await response.SendFileAsync(fileName);
            }
        );
    }
}
