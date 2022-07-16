using System.Globalization;

namespace EmploymentAgency.Utils;

public abstract class CrudAction
{
    private readonly Dictionary<string, IEnumerable<IEnumerable<byte>>> _fileSignatures;
    private readonly int _maxFileSizeInBytes;

    public CrudAction(
        Dictionary<string, IEnumerable<IEnumerable<string>>> fileSignatures,
        int maxFileSizeInBytes
    )
    {
        _fileSignatures = fileSignatures.ToDictionary(
            entry => entry.Key,
            entry =>
                entry.Value.Select(
                    sigs => sigs.Select(sig => byte.Parse(sig, NumberStyles.HexNumber))
                )
        );
        _maxFileSizeInBytes = maxFileSizeInBytes;
    }

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
        var imageFile = request.Form.Files.SingleOrDefault();

        if (imageFile is null)
        {
            return Table(table, request);
        }

        string extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!_fileSignatures.Keys.Contains(extension))
        {
            throw new ArgumentException("Invalid file extension.");
        }

        if (imageFile.Length > _maxFileSizeInBytes)
        {
            throw new ArgumentException("File is too large.");
        }

        using (Stream fileStream = imageFile.OpenReadStream())
        {
            if (HasInvalidSignature(fileStream, extension, _fileSignatures[extension]))
            {
                throw new ArgumentException($"File content does not match it's extension.");
            }
        }

        string imageFilePath = $"media/photos/user{Guid.NewGuid().ToString()}{extension}";
        using var stream = File.Create(imageFilePath);
        imageFile.CopyToAsync(stream).Wait();
        return PerformOnTable(
            table,
            request.Form.Select(x => x.Key).Append("photo"),
            request.Form
                .Select(x => x.Value.ToString())
                .Append(imageFilePath[(imageFilePath.LastIndexOf('/') + 1)..])
        );
    }

    protected string DefaultIfEmpty(string value)
    {
        return value == "" ? "DEFAULT" : $"'{value}'";
    }

    private bool HasInvalidSignature(
        Stream file,
        string expectedExtension,
        IEnumerable<IEnumerable<byte>> signatures
    )
    {
        using var reader = new BinaryReader(file);
        int longestSignatureLength = signatures.Max(sig => sig.Count());
        var headerBytes = reader.ReadBytes(longestSignatureLength);

        return signatures.Any(sig => headerBytes.Take(sig.Count()).SequenceEqual(sig));
    }
}
