using System.Text;
using EmploymentAgency;
using EmploymentAgency.EndpointMappers;
using EmploymentAgency.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

Settings settings = GetSettings();
WebApplication app = BuildApp();
UseRequiredMiddlewares();

PostgreSql postgres = MakePostgres();
MapAllEndpoints();

app.Run();

Settings GetSettings()
{
    return new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build()
        .GetSection("Settings")
        .Get<Settings>();
}

WebApplication BuildApp()
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
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
    return builder.Build();
}

void UseRequiredMiddlewares()
{
    app.UseHttpsRedirection();
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
        settings.RetryDelayMultiplier
    );
    return new PostgreSql(settings.ConnectionString, retry);
}

void MapAllEndpoints()
{
    CrudQueriesMapper.Map(app, postgres);
    SpecialQueriesMapper.Map(app, postgres);
    FileMapper.Map(app);
    LoginMapper.Map(app, GetSecurityKey, settings);
    app.MapGet(
        "api/generate",
        async () =>
        {
            string command = await File.ReadAllTextAsync("../sql/employment_agency.sql");
            await postgres.ExecuteAsync(command);
        }
    );
}

SymmetricSecurityKey GetSecurityKey()
{
    return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret));
}
