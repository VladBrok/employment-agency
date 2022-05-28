using EmploymentAgency;
using EmploymentAgency.EndpointMappers;
using EmploymentAgency.Models;

/**
    * Performance
    * Security
    * Error handling
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
    builder.Services.AddCors();

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
    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod());
}

PostgreSql MakePostgres()
{
    var settings = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build()
        .GetSection("Settings")
        .Get<Settings>();
    var retry = new RetryStrategy(
        settings.MaxRetryCount,
        settings.InitialRetryDelayMs,
        settings.RetryDelayMultiplier
    );
    return new PostgreSql(settings.ConnectionString, retry);
}

void MapAllEndpoints()
{
    CrudQueriesMapper.Map(app, postgres);
    SpecialQueriesMapper.Map(app, postgres);
    FileMapper.Map(app);
}
