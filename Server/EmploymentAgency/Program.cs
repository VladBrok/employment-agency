using System.Text;
using EmploymentAgency;
using EmploymentAgency.EndpointMappers;
using EmploymentAgency.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Settings settings = GetSettings();
WebApplication app = BuildApp();
UseRequiredMiddlewares();

PostgreSql postgres = MakePostgres();
MapAllEndpoints();

app.Run();

Settings GetSettings()
{
    return builder.Configuration.GetSection("Settings").Get<Settings>();
}

WebApplication BuildApp()
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(
            options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    IssuerSigningKey = GetSecurityKey(),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            }
        );
    builder.Services.AddAuthorization(
        options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build()
    );
    builder.Services.AddCors();
    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration.ReadFrom
                .Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );
    builder.Services.AddLogging(builder => builder.AddSerilog());
    return builder.Build();
}

void UseRequiredMiddlewares()
{
    app.UseHttpsRedirection();

    string logFolder = "logs";
    string logPath = Path.Combine(builder.Environment.ContentRootPath, logFolder);
    Directory.CreateDirectory(logPath);

    app.UseStaticFiles(
        new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(logPath),
            RequestPath = $"/{logFolder}"
        }
    );
    app.UseSerilogRequestLogging();
    app.UseCors(
        builder =>
            builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
    );
    app.UseAuthentication();
    app.UseAuthorization();
}

PostgreSql MakePostgres()
{
    var retry = new RetryStrategy(
        settings.MaxRetryCount,
        settings.InitialRetryDelayMs,
        settings.RetryDelayMultiplier,
        app.Logger
    );
    return new PostgreSql(settings.ConnectionString, retry, app.Logger);
}

void MapAllEndpoints()
{
    CrudQueriesMapper.Map(app, postgres, settings);
    SpecialQueriesMapper.Map(app, postgres);
    FileMapper.Map(app);
    LoginMapper.Map(app, GetSecurityKey, settings);
    app.MapGet(
        "api/generate",
        async () =>
        {
            string command = await File.ReadAllTextAsync("sql/employment_agency.sql");
            await postgres.ExecuteAsync(command);
        }
    );
}

SymmetricSecurityKey GetSecurityKey()
{
    return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret));
}
