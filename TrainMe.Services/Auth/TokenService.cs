using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private DateTime _expires;

    public TokenService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public string CreateAccessToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var jwtConfig = GetJwtConfiguration();
        var signingCredentials = CreateSigningCredentials(jwtConfig.Key);

        _expires = DateTime.UtcNow.AddMinutes(jwtConfig.ExpireMinutes);

        var claims = CreateClaims(user);
        var token = CreateJwtToken(jwtConfig, claims, signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpiration() => _expires;

    private (string Key, string? Issuer, string? Audience, int ExpireMinutes) GetJwtConfiguration()
    {
        var jwt = _config.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key configuration");
        var expireMinutes = int.TryParse(jwt["ExpireMinutes"], out var minutes) ? minutes : 30;

        return (key, jwt["Issuer"], jwt["Audience"], expireMinutes);
    }

    private static SigningCredentials CreateSigningCredentials(string key)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    private static List<Claim> CreateClaims(User user) => new()
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.UniqueName, user.UserName),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Role, user.Role)
    };

    private JwtSecurityToken CreateJwtToken(
        (string Key, string? Issuer, string? Audience, int ExpireMinutes) config,
        List<Claim> claims,
        SigningCredentials credentials) => new(
            issuer: config.Issuer,
            audience: config.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: _expires,
            signingCredentials: credentials);
}
