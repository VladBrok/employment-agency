using EmploymentAgency.Reports;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentAgency.EndpointMappers;

public static class FileMapper
{
    private static readonly HashSet<string> _allowedExtensions = new() { ".png", ".jpg", ".jpeg" };

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
                var report = new ReportFactory().MakeReport(type);
                await report.BuildAsync(entities, fileName, title);
                await SendFileAsync(fileName, response, "application/octet-stream");
            }
        );

        app.MapGet(
            "api/photos/{fileName}",
            async (string fileName, HttpResponse response) =>
            {
                bool containsDir = fileName.IndexOf("/") != -1;
                string file = containsDir ? fileName : $"photos/{fileName}";
                await SendFileAsync(file, response, "image/png");
            }
        );

        app.MapPost(
            "form/test",
            async (HttpRequest request) =>
            {
                var data = request.Form;
                foreach (var item in data)
                {
                    Console.WriteLine(item.Key + " " + item.Value);
                }

                var imageFile = request.Form.Files.SingleOrDefault();
                if (imageFile is null)
                {
                    return Results.Ok();
                }
                if (!_allowedExtensions.Contains(Path.GetExtension(imageFile.FileName)))
                {
                    return Results.BadRequest(imageFile.Name);
                }

                using var stream = File.Create("photos/user.png");
                await imageFile.CopyToAsync(stream);
                return Results.Ok();
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
