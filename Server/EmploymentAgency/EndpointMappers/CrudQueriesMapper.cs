namespace EmploymentAgency.EndpointMappers;

public static class CrudQueriesMapper
{
    private static PostgreSql _postgres;

    public static void Map(WebApplication app, PostgreSql postgres)
    {
        _postgres = postgres;
        var mainTables = new (string Name, string Command, string? Alias, string[]? ParentIds)[]
        {
            ("addresses", Select.FromAddresses(), "a", new[] { "street_id" }),
            (
                "applications",
                Select.FromApplications(),
                "a",
                new[] { "seeker_id", "position_id", "employment_type_id" }
            ),
            ("employers", Select.FromEmployers(), "e", new[] { "property_id", "address_id" }),
            (
                "seekers",
                Select.FromSeekers(),
                "s",
                new[] { "status_id", "address_id", "speciality_id" }
            ),
            ("streets", Select.FromStreets(), "s", new[] { "district_id" }),
            ("vacancies", Select.FromVacancies(), "v", new[] { "employer_id", "position_id" }),
        };
        var referenceTableNames = new[]
        {
            "change_log",
            "districts",
            "employment_types",
            "positions",
            "properties",
            "statuses"
        };
        var allTables = mainTables.Concat(
            referenceTableNames.Select<
                string,
                (string Name, string Command, string? Alias, string[]? ParentIds)
            >(n => (n, Select.From(n), null, null))
        );

        foreach (var table in mainTables)
        {
            foreach (var parentId in table.ParentIds)
            {
                app.MapGet(
                    $"{getEndpoint(table.Name)}/{parentId}/{{id}}",
                    async (int page, int pageSize, string? filter, int id) =>
                    {
                        var command = $"{table.Command} WHERE {table.Alias}.{parentId} = {id}";
                        return await postgres.ReadPageAsync(page, pageSize, command, filter);
                    }
                );
            }
        }

        foreach (var table in allTables)
        {
            app.MapGet(
                getEndpoint(table.Name),
                async (int page, int pageSize, string? filter) =>
                    await postgres.ReadPageAsync(page, pageSize, table.Command, filter)
            );
            app.MapGet(
                getEndpointWithId(table.Name),
                async (int id) => await ReadSingleAsync(id, table.Alias, table.Command)
            );

            app.MapPost(
                getEndpoint(table.Name),
                async (Entity entity) =>
                {
                    await postgres.ExecuteAsync(
                        $@"INSERT INTO {table.Name} ({string.Join(", ", entity.Keys)}) 
                        VALUES ('{string.Join("', '", entity.Values)}')"
                    );
                }
            );
            app.MapPut(
                getEndpointWithId(table.Name),
                async (int id, Entity entity) =>
                    await ReadSingleAsync(
                        id,
                        null,
                        $@"UPDATE {table.Name} 
                        SET {string.Join(", ", entity.Select(e => $"{e.Key} = '{e.Value}'"))}"
                    )
            );
            app.MapDelete(
                getEndpointWithId(table.Name),
                async (int id) => await ReadSingleAsync(id, null, $@"DELETE FROM {table.Name}")
            );
        }

        string getEndpoint(string table) => $"api/{table}";
        string getEndpointWithId(string table) => $"{getEndpoint(table)}/{{id}}";
    }

    private static async Task<IResult> ReadSingleAsync(int id, string? tableAlias, string command)
    {
        Entity? result = await _postgres.ReadSingleAsync(id, tableAlias, command);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
