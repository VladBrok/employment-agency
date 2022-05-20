using EmploymentAgency;
using EmploymentAgency.EndpointMappers;
using EmploymentAgency.Models;

/**
    * В адресе должна быть улица, иначе появляются дубликаты при запросе seekers
    * Timeouts and error handling
    * Get by parent id (select * from applications where seeker_id = 1)
    
    * Performance
    * Security
    * update или delete нарушает ограничение внешнего клуча
*/

WebApplication app = BuildApp();
UseRequiredMiddlewares();

PostgreSql postgres = MakePostgres();
MapAllEndpoints();

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

void MapAllEndpoints()
{
    CrudQueriesMapper.Map(app, postgres);
    SpecialQueriesMapper.Map(app, postgres);
    ReportsMapper.Map(app);
}
