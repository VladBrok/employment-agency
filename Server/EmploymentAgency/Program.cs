using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmploymentAgency;
using EmploymentAgency.EndpointMappers;
using EmploymentAgency.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

/**
    * Error handling
    * Performance
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

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(
            options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    IssuerSigningKey = GetSymmetricSecurityKey(),
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

    app.MapPost(
        "api/login",
        [AllowAnonymous]
        (User user) =>
        {
            if (user.Login != "admin")
            {
                return Results.BadRequest(new { error = "Неверный логин" });
            }
            if (user.Password != "1234")
            {
                return Results.BadRequest(new { error = "Неверный пароль" });
            }

            var claims = new List<Claim> { new(ClaimsIdentity.DefaultNameClaimType, user.Login), };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                null
            );

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: now.AddDays(1),
                signingCredentials: new SigningCredentials(
                    GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256
                )
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new { access_token = encodedJwt, login = user.Login };

            return Results.Ok(response);
        }
    );
}

SymmetricSecurityKey GetSymmetricSecurityKey()
{
    return new SymmetricSecurityKey(
        Encoding.ASCII.GetBytes("secretsecretsecretsecretsecretsecretsecretsecretsecret")
    );
}

record User(string Login, string Password);
