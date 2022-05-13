using System.Data;
using Npgsql;

namespace EmploymentAgency.Services;

public class PostgreSql
{
    private readonly string _connection;

    public PostgreSql(string connection)
    {
        _connection = connection;
    }

    public async Task<int> ExecuteReaderAsync(string command)
    {
        await using var conn = new NpgsqlConnection(_connection);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(command, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
        return await Task.Run(() => 2);
    }
}
