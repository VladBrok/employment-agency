using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Threading.Tasks;
using EmploymentAgency.Models;

/**
    * update или delete нарушает ограничение внешнего клуча
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
    var mainTables = new (string Name, ReadAsyncFunc ReadAsync)[] {
        ("addresses", ReadAddressesAsync),
        ("applications", ReadApplicationsAsync),
        ("employers", ReadEmployersAsync),
        ("seekers", ReadSeekersAsync),
        ("streets", ReadStreetsAsync),
        ("vacancies", ReadVacanciesAsync),
    };
    var referenceTableNames =
        new[] { "change_log", "districts", "employment_types", "positions", "properties", "statuses" };

    foreach (var table in mainTables
                            .Concat(referenceTableNames
                                .Select<string, (string Name, ReadAsyncFunc ReadAsync)>(n => (
                                    n,
                                    (int page, int pageSize, int? id) => ReadAsync(page, pageSize, n, id)))))
    {
        app.MapGet(
            getEndpoint(table.Name),
            async (int page, int pageSize) => await table.ReadAsync(page, pageSize));

        app.MapGet(
            getEndpointWithId(table.Name),
            async (int id) => await table.ReadAsync(0, 1, id));

        app.MapPost(getEndpoint(table.Name), async (Entity entity) =>
        {
            await postgres.ExecuteAsync(
                $@"INSERT INTO {table.Name} ({string.Join(", ", entity.Keys)}) 
               VALUES ('{string.Join("', '", entity.Values)}')");
        });

        app.MapPut(getEndpointWithId(table.Name), async (int id, Entity entity) =>
        {
            await postgres.ExecuteAsync(
                $@"UPDATE {table.Name} 
               SET {string.Join(", ", entity.Select(e => $"{e.Key} = '{e.Value}'"))}
               WHERE id = {id}");
        });

        app.MapDelete(getEndpointWithId(table.Name), async (int id) =>
        {
            await postgres.ExecuteAsync(
                $@"DELETE FROM {table.Name} 
               WHERE id = {id}");
        });
    }

    string getEndpoint(string table) => $"api/{table}";
    string getEndpointWithId(string table) => $"{getEndpoint(table)}/{{id}}";
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


async Task<IResult> ReadEmployersAsync(int page, int pageSize, int? id = null)
{
    return await ReadPageAsync(page, pageSize,
        $@"SELECT e.id, e.employer, p.property, 
                  e.phone, e.email, d.district, 
                  s.street, a.building_number, s.postal_code
           FROM employers e
           JOIN properties p ON p.id = e.property_id
           JOIN addresses a ON a.id = e.address_id
           JOIN districts d ON d.id = a.district_id
           JOIN streets s ON d.id = s.district_id", id, "e");
}

async Task<IResult> ReadSeekersAsync(int page, int pageSize, int? id = null)
{
    return await ReadPageAsync(page, pageSize,
        $@"SELECT s.id, s.first_name, s.last_name, s.patronymic,
                  stat.status, p.position, s.education
                  s.phone, s.birthday, s.registration_city,
                  s.recommended, s.pol, d.district,
                  st.street, a.building_number, st.postal_code
           FROM seekers s
           JOIN statuses stat ON stat.id = s.status_id
           JOIN positions p ON p.id = s.speciality_id
           JOIN addresses a ON a.id = s.address_id
           JOIN districts d ON d.id = a.district_id
           JOIN streets st ON d.id = st.district_id", id, "s");
}

async Task<IResult> ReadAddressesAsync(int page, int pageSize, int? id = null)
{
    return await ReadPageAsync(page, pageSize,
        $@"SELECT a.id, d.district, s.street, a.building_number, s.postal_code
           FROM addresses a
           JOIN districts d ON d.id = a.district_id
           JOIN streets s ON d.id = s.district_id", id, "a");
}

async Task<IResult> ReadVacanciesAsync(int page, int pageSize, int? id = null)
{
    return await ReadPageAsync(page, pageSize,
        $@"SELECT v.id, v.employer_id, p.position, v.employer_day,
                  v.salary_new, v.chart_new, v.vacancy_end
           FROM vacancies v
           JOIN positions p ON p.id = v.position_id", id, "v");
}

async Task<IResult> ReadApplicationsAsync(int page, int pageSize, int? id = null)
{
    return await ReadPageAsync(page, pageSize,
        $@"SELECT a.id, a.seeker_id, p.position, et.type,
                  a.seeker_day, a.information, a.photo, a.experience
           FROM applications a
           JOIN positions p ON p.id = a.position_id
           JOIN employment_types et ON et.id = a.employment_type_id", id, "a");
}

async Task<IResult> ReadStreetsAsync(int page, int pageSize, int? id = null)
{
    return await ReadPageAsync(page, pageSize,
        $@"SELECT s.id, d.district, s.street, s.postal_code
           FROM streets s
           JOIN districts d ON d.id = s.district_id", id, "s");
}

async Task<IResult> ReadAsync(int page, int pageSize, string target, int? id = null)
{
    return await ReadPageAsync(page, pageSize, $@"SELECT * FROM {target}", id);
}

async Task<IResult> ReadPageAsync(int page, int pageSize, string command, int? id = null, string? tableAlias = null)
{
    tableAlias = tableAlias is null ? "" : tableAlias + ".";
    return Results.Ok(await postgres.ExecuteAsync(
        $@"{command}
           {(id is null ? "" : $"WHERE {tableAlias}id = {id}")}
           OFFSET {page * pageSize}
           LIMIT {pageSize}; "
    ));
}

delegate Task<IResult> ReadAsyncFunc(int page, int pageSize, int? id = null);
