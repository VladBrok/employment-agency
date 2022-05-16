using Npgsql;

public class PostgreSql
{
    private readonly string _connection;

    public PostgreSql(string connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Entity>> ExecuteAsync(
        string command)
    {
        Console.WriteLine(command);

        await using var conn = new NpgsqlConnection(_connection);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(command, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        var result = new List<Entity>();

        while (await reader.ReadAsync())
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            int columnIndex = 0;
            result.Add(values.ToDictionary(
                _ => reader.GetName(columnIndex++),
                v => v.ToString() ?? ""));
        }

        return result;
    }
}
