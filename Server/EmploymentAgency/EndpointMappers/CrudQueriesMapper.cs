namespace EmploymentAgency.EndpointMappers;

public static class CrudQueriesMapper
{
    private static PostgreSql _postgres;

    public static void Map(WebApplication app, PostgreSql postgres)
    {
        _postgres = postgres;
        var mainTables = new (string Name, string Command, string? Alias)[] {
            ("addresses", Select.FromAddresses(), "a"),
            ("applications", Select.FromApplications(), "a"),
            ("employers", Select.FromEmployers(), "e"),
            ("seekers", Select.FromSeekers(), "s"),
            ("streets", Select.FromStreets(), "s"),
            ("vacancies", Select.FromVacancies(), "v"),
        };
        var referenceTableNames =
            new[] { "change_log", "districts", "employment_types", "positions", "properties", "statuses" };

        foreach (var table in mainTables.Concat(referenceTableNames
                                .Select<string, (string Name, string Command, string? Alias)>(
                                    n => (n, Select.From(n), null))))
        {
            app.MapGet(
                getEndpoint(table.Name),
                async (int page, int pageSize, string? filter) =>
                    await postgres.ReadPageAsync(page, pageSize, table.Command, filter));

            app.MapGet(
                getEndpointWithId(table.Name),
                async (int id) =>
                    await ReadSingleAsync(id, table.Alias, table.Command));

            app.MapPost(getEndpoint(table.Name), async (Entity entity) =>
            {
                await postgres.ExecuteAsync(
                    $@"INSERT INTO {table.Name} ({string.Join(", ", entity.Keys)}) 
                    VALUES ('{string.Join("', '", entity.Values)}')");
            });

            app.MapPut(getEndpointWithId(table.Name), async (int id, Entity entity) =>
                await ReadSingleAsync(
                    id,
                    null,
                    $@"UPDATE {table.Name} 
                    SET {string.Join(", ", entity.Select(e => $"{e.Key} = '{e.Value}'"))}"));

            app.MapDelete(getEndpointWithId(table.Name), async (int id) =>
                await ReadSingleAsync(id, null, $@"DELETE FROM {table.Name}"));
        }

        string getEndpoint(string table) => $"api/{table}";
        string getEndpointWithId(string table) => $"{getEndpoint(table)}/{{id}}";
    }

    private static async Task<IResult> ReadSingleAsync(
        int id,
        string? tableAlias,
        string command)
    {
        Entity? result = await _postgres.ReadSingleAsync(id, tableAlias, command);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
