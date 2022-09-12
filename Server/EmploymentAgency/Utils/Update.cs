namespace EmploymentAgency.Utils;

public class Update : CrudAction
{
    public Update(
        Dictionary<string, IEnumerable<IEnumerable<string>>> fileSignatures,
        int maxFileSize
    ) : base(fileSignatures, maxFileSize) { }

    protected override string PerformOnTable(
        string table,
        IEnumerable<string> keys,
        IEnumerable<string> values
    )
    {
        return $@"UPDATE {table} 
        SET {string.Join(", ", keys.Zip(values).Select(e => $"{Escape(e.First)} = {DefaultIfEmpty(Escape(e.Second))}"))}";
    }
}
