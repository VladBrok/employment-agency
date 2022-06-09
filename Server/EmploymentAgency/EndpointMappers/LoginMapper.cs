using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmploymentAgency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace EmploymentAgency.EndpointMappers;

public static class LoginMapper
{
    public static void Map(
        WebApplication app,
        Func<SymmetricSecurityKey> GetSecurityKey,
        Settings settings
    )
    {
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

                var claims = new List<Claim>
                {
                    new(ClaimsIdentity.DefaultNameClaimType, user.Login),
                };
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
                        GetSecurityKey(),
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

    private static string Hash(string str)
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
}
