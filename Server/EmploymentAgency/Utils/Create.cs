namespace EmploymentAgency.Utils;

public class Create : CrudAction
{
    public Create(
        Dictionary<string, IEnumerable<IEnumerable<string>>> fileSignatures,
        int maxFileSize
    ) : base(fileSignatures, maxFileSize) { }

    protected override string PerformOnTable(
        string table,
        IEnumerable<string> keys,
        IEnumerable<string> values
    )
    {
        return $@"INSERT INTO {table} ({Escape(string.Join(", ", keys))}) 
        VALUES ({string.Join(", ", values.Select(v => DefaultIfEmpty(Escape(v))))})";
    }
}
