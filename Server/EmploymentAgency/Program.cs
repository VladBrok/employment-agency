using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmploymentAgency;
using EmploymentAgency.EndpointMappers;
using EmploymentAgency.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
            if (user.Login != settings.Admin.Login)
            {
                return Results.BadRequest(new { error = "Неверный логин" });
            }
            if (Hash(user.Password) != settings.Admin.Password)
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

            DateTime now = DateTime.UtcNow;
            DateTime expirationDate = now.AddMilliseconds(settings.JwtLifetimeMs);
            var jwt = new JwtSecurityToken(
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: expirationDate,
                signingCredentials: new SigningCredentials(
                    GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256
                )
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                login = user.Login,
                expires = new DateTimeOffset(expirationDate).ToUnixTimeMilliseconds()
            };

            return Results.Ok(response);
        }
    );
}

SymmetricSecurityKey GetSymmetricSecurityKey()
{
    return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret));
}

string Hash(string str)
{
    byte[] derivedKey = KeyDerivation.Pbkdf2(
        str,
        salt: Array.Empty<byte>(),
        KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8
    );
    return Convert.ToBase64String(derivedKey);
}
