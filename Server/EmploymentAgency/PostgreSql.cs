using Npgsql;

namespace EmploymentAgency;

public class PostgreSql
{
    private readonly string _connection;
    private readonly RetryStrategy _retry;

    public PostgreSql(string connection, RetryStrategy retry)
    {
        _connection = connection;
        _retry = retry;
    }

    public async Task<IEnumerable<Entity>> ExecuteAsync(string command)
    {
        var result = new List<Entity>();

        await ExecuteReaderAsync(
            command,
            entity =>
            {
                result.Add(entity);
                return false;
            }
        );
        return result;
    }

    public async Task<Entity?> ReadSingleAsync(int id, string? tableAlias, string command)
    {
        command =
            $@"{command}
            WHERE {(tableAlias is null ? "" : $"{tableAlias}.")}id = {id}";

        int count = await ExecuteNonQueryAsync(command);
        if (count == 0)
        {
            return null;
        }

        Entity result = new();
        await ExecuteReaderAsync(
            command,
            entity =>
            {
                result = entity;
                return true;
            }
        );
        return result;
    }

    public async Task<IEnumerable<Entity>> ReadPageAsync(
        int page,
        int pageSize,
        string command,
        string? filter = null
    )
    {
        if (filter is null)
        {
            command =
                $@"{command}
                OFFSET {page * pageSize}
                LIMIT {pageSize};";
            return await ExecuteAsync(command);
        }

        var entities = await ExecuteAsync(command);
        return entities
            .Where(
                e =>
                    e.Keys.Any(
                        k =>
                            !k.EndsWith("id")
                            && e[k].Contains(filter, StringComparison.OrdinalIgnoreCase)
                    )
            )
            .Skip(page * pageSize)
            .Take(pageSize);
    }

    private async Task ExecuteReaderAsync(string command, Func<Entity, bool> callback)
    {
        Console.WriteLine(command);
        await _retry.ExecuteAsync<NpgsqlException>(
            async () =>
            {
                await using var conn = new NpgsqlConnection(_connection);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(command, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                bool shouldStop = false;
                while (!shouldStop && await reader.ReadAsync())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);

                    int columnIndex = 0;
                    shouldStop = callback(
                        values.ToDictionary(
                            _ => reader.GetName(columnIndex++),
                            v => v.ToString() ?? ""
                        )
                    );
                }
            },
            exception => exception.IsTransient
        );
    }

    private async Task<int> ExecuteNonQueryAsync(string command)
    {
        int count = 0;
        await _retry.ExecuteAsync<NpgsqlException>(
            async () =>
            {
                await using var conn = new NpgsqlConnection(_connection);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(command, conn);
                count = await cmd.ExecuteNonQueryAsync();
            },
            exception => exception.IsTransient
        );
        return count;
    }
}
