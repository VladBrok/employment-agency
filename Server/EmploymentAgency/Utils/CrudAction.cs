namespace EmploymentAgency.Utils;

public abstract class CrudAction
{
    private readonly HashSet<string> _allowedExtensions = new() { ".png", ".jpg", ".jpeg" };

    public string Table(string table, HttpRequest request)
    {
        return PerformOnTable(
            table,
            request.Form.Select(x => x.Key),
            request.Form.Select(x => x.Value.ToString())
        );
    }

    protected abstract string PerformOnTable(
        string table,
        IEnumerable<string> keys,
        IEnumerable<string> values
    );

    public virtual string Applications(string table, HttpRequest request)
    {
        string imageFilePath = $"media/photos/user{Guid.NewGuid().ToString()}.png";
        var imageFile = request.Form.Files.SingleOrDefault();

        if (imageFile is not null)
        {
            if (!_allowedExtensions.Contains(Path.GetExtension(imageFile.FileName)))
            {
                imageFilePath = "";
            }
            else
            {
                using var stream = File.Create(imageFilePath);
                imageFile.CopyToAsync(stream).Wait();
            }
            return PerformOnTable(
                table,
                request.Form.Select(x => x.Key).Append("photo"),
                request.Form
                    .Select(x => x.Value.ToString())
                    .Append(imageFilePath[(imageFilePath.LastIndexOf('/') + 1)..])
            );
        }

        return Table(table, request);
    }

    protected string DefaultIfEmpty(string value)
    {
        return value == "" ? "DEFAULT" : $"'{value}'";
    }
}
