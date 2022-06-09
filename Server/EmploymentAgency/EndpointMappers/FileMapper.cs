using EmploymentAgency.Reports;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentAgency.EndpointMappers;

public static class FileMapper
{
    public static void Map(WebApplication app)
    {
        app.MapPost(
            "api/reports/{type}",
            async (
                string fileName,
                string type,
                string title,
                [FromBody] IEnumerable<Entity> entities,
                HttpResponse response
            ) =>
            {
                var report = ReportFactory.MakeReport(type);
                string file = $"media/reports/{fileName}";
                await report.BuildAsync(entities, file, title);
                await SendFileAsync(file, response, "application/octet-stream");
            }
        );

        app.MapGet(
            "api/photos/{fileName}",
            async (string fileName, HttpResponse response) =>
            {
                await SendFileAsync($"media/photos/{fileName}", response, "image/png");
            }
        );
    }

    private static async Task SendFileAsync(string file, HttpResponse response, string contentType)
    {
        response.Headers.Add("content-disposition", $"attachment;filename={file}");
        response.ContentType = contentType;
        await response.SendFileAsync(file);
    }
}
