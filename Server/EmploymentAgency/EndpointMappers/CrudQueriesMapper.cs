namespace EmploymentAgency.EndpointMappers;

using EmploymentAgency.Utils;
using EmploymentAgency.Models;

public static class CrudQueriesMapper
{
    private static PostgreSql? _postgres;

    public static void Map(WebApplication app, PostgreSql postgres, Settings settings)
    {
        _postgres = postgres;
        var update = new Update(settings.FileSignatures, settings.MaxFileSizeInBytes);
        var create = new Create(settings.FileSignatures, settings.MaxFileSizeInBytes);
        var mainTables =
            new (string Name, string Select, Delegate Update, Delegate Create, string? Alias, string[]? ParentIds)[]
            {
                (
                    "applications",
                    Select.FromApplications(),
                    update.Applications,
                    create.Applications,
                    "a",
                    new[] { "seeker_id", "position_id", "employment_type_id" }
                ),
                (
                    "employers",
                    Select.FromEmployers(),
                    update.SeekersOrEmployers,
                    create.SeekersOrEmployers,
                    "e",
                    new[] { "property_id", "district_id" }
                ),
                (
                    "seekers",
                    Select.FromSeekers(),
                    update.SeekersOrEmployers,
                    create.SeekersOrEmployers,
                    "s",
                    new[] { "status_id", "district_id", "education_id", "registration_city_id" }
                ),
                (
                    "vacancies",
                    Select.FromVacancies(),
                    update.Table,
                    create.Table,
                    "v",
                    new[] { "employer_id", "position_id" }
                ),
                (
                    "districts",
                    Select.FromDistricts(),
                    update.Table,
                    create.Table,
                    "d",
                    new[] { "city_id" }
                ),
            };
        var referenceTableNames = new[]
        {
            "change_log",
            "cities",
            "educations",
            "employment_types",
            "positions",
            "properties",
            "statuses"
        };
        var allTables = mainTables.Concat(
            referenceTableNames.Select<
                string,
                (string Name, string Select, Delegate Update, Delegate Create, string? Alias, string[]? ParentIds)
            >(n => (n, Select.From(n), update.Table, create.Table, null, null))
        );

        foreach (var table in mainTables)
        {
            foreach (var parentId in table.ParentIds)
            {
                app.MapGet(
                    $"{getEndpoint(table.Name)}/{parentId}/{{id}}",
                    async (int page, int pageSize, string? filter, int id) =>
                    {
                        var command = $"{table.Select} WHERE {table.Alias}.{parentId} = {id}";
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
                    await postgres.ReadPageAsync(page, pageSize, table.Select, filter)
            );
            app.MapGet(
                getEndpointWithId(table.Name),
                async (int id) => await ReadSingleAsync(id, table.Alias, table.Select)
            );

            app.MapPost(
                getEndpoint(table.Name),
                async (HttpRequest request) =>
                    await postgres.ExecuteAsync(
                        table.Create.DynamicInvoke(table.Name, request).ToString()
                    )
            );
            app.MapPut(
                getEndpointWithId(table.Name),
                async (int id, HttpRequest request) =>
                    await ReadSingleAsync(
                        id,
                        null,
                        table.Update.DynamicInvoke(table.Name, request).ToString()
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
        Entity? result = await _postgres!.ReadSingleAsync(id, tableAlias, command);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
