using EmploymentAgency.Models;

/**
    * update или delete нарушает ограничение внешнего клуча
    * Read
    * Reports
    * Timeouts and error handling
    
    * Performance
    * Security
*/

WebApplication app = BuildApp();
UseRequiredMiddlewares();

PostgreSql postgres = MakePostgres();
MapCrudQueries();
MapSpecialQueries(root: "api/special");

app.Run();


WebApplication BuildApp()
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    return builder.Build();
}

void UseRequiredMiddlewares()
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
}

PostgreSql MakePostgres()
{
    var settings = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build()
        .GetSection("Settings")
        .Get<Settings>();
    return new PostgreSql(settings.ConnectionString);
}

void MapCrudQueries()
{
    foreach (string table in new[]
    {
        "addresses",
        "applications",
        "districts",
        "employers",
        "employment_types",
        "positions",
        "properties",
        "seekers",
        "statuses",
        "streets",
        "vacancies",
    })
    {
        string endpoint = $"api/{table}";

        app.MapPost(endpoint, async (Entity entity) =>
        {
            await postgres.ExecuteAsync(
                $@"INSERT INTO {table} ({string.Join(", ", entity.Keys)}) 
               VALUES ('{string.Join("', '", entity.Values)}')");
        });

        app.MapPut($"{endpoint}/{{id}}", async (int id, Entity entity) =>
        {
            await postgres.ExecuteAsync(
                $@"UPDATE {table} 
               SET {string.Join(", ", entity.Select(e => $"{e.Key} = '{e.Value}'"))}
               WHERE id = {id}");
        });

        app.MapDelete($"{endpoint}/{{id}}", async (int id) =>
        {
            await postgres.ExecuteAsync(
                $@"DELETE FROM {table} 
               WHERE id = {id}");
        });
    }
}

void MapSpecialQueries(string root)
{
    foreach (string function in new[]
    {
        "average_seeker_ages_by_positions",
        "employer_addresses",
        "employment_types_and_salaries",
        "vacancies_and_salaries",
        "employers_and_vacancies",
        "seekers_and_applications",
        "num_vacancies_from_each_employer"
    })
    {
        app.MapGet(
            $"{root}/{function}",
            async (int page, int pageSize) =>
                await ReadAsync(page, pageSize, function));
    }

    app.MapGet(
        $"{root}/applications_percent_after",
        async (int page, int pageSize, int year) =>
            await ReadAsync(page, pageSize, $"get_applications_percent_after('{year}')"));

    app.MapGet(
        $"{root}/applications_percent_by_positions_after",
        async (int page, int pageSize, int year) =>
            await ReadAsync(page, pageSize, $"get_applications_percent_by_positions_after('{year}')"));

    app.MapGet(
        $"{root}/application_count_by_positions",
        async (int page, int pageSize, int year, int month) =>
            await ReadAsync(page, pageSize, $"get_application_count_by_positions('{year}', '{month}')"));

    app.MapGet(
        $"{root}/seekers_in_district",
        async (int page, int pageSize, string district) =>
            await ReadAsync(page, pageSize, $"get_seekers_in_district('{district}')"));

    app.MapGet(
        $"{root}/employers_with_property",
        async (int page, int pageSize, string property) =>
            await ReadAsync(page, pageSize, $"get_employers_with_property('{property}')"));

    app.MapGet(
        $"{root}/vacancies_posted_on",
        async (int page, int pageSize, DateOnly date) =>
            await ReadAsync(page, pageSize, $"get_vacancies_posted_on('{date}')"));

    app.MapGet(
        $"{root}/seekers_born_after",
        async (int page, int pageSize, DateOnly date) =>
            await ReadAsync(page, pageSize, $"get_seekers_born_after('{date}')"));

    app.MapGet(
        $"{root}/applications_without_experience",
        async (int page, int pageSize, string position) =>
            await ReadAsync(page, pageSize, $"get_applications_without_experience('{position}')"));

    app.MapGet(
        $"{root}/max_salaries_for_position",
        async (int page, int pageSize, string position) =>
            await ReadAsync(page, pageSize, $"get_max_salaries_for_position('{position}')"));

    app.MapGet(
        $"{root}/seekers_whose_total_experience_exceeds",
        async (int page, int pageSize, int experience) =>
            await ReadAsync(page, pageSize, $"get_seekers_whose_total_experience_exceeds('{experience}')"));

    app.MapGet(
        $"{root}/positions_from_open_vacancies_whose_average_salary_exceeds",
        async (int page, int pageSize, int salary) =>
            await ReadAsync(page, pageSize, $"get_positions_from_open_vacancies_whose_average_salary_exceeds('{salary}')"));

    app.MapGet(
        $"{root}/num_applications_for_each_employment_type",
        async (int page, int pageSize) =>
            await ReadAsync(page, pageSize, $"get_num_applications_for_each_employment_type()"));

    app.MapGet(
        $"{root}/latest_vacancy_of_employers_whose_name_contains",
        async (int page, int pageSize, string pattern) =>
            await ReadAsync(page, pageSize, $"get_latest_vacancy_of_employers_whose_name_contains('{{pattern}}')"));
}

async Task<IResult> ReadAsync(int page, int pageSize, string target)
{
    return Results.Ok(await postgres.ExecuteAsync(
        $@"SELECT * FROM {target} 
           OFFSET {page * pageSize}
           LIMIT {pageSize};"));
}
