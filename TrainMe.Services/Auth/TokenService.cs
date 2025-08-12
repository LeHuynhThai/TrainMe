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
        var config = GetJwtConfiguration();
        _expires = DateTime.UtcNow.AddMinutes(config.ExpireMinutes);

        var token = new JwtSecurityToken(
            config.Issuer, config.Audience, CreateClaims(user),
            DateTime.UtcNow, _expires, CreateSigningCredentials(config.Key));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpiration() => _expires;

    private (string Key, string? Issuer, string? Audience, int ExpireMinutes) GetJwtConfiguration()
    {
        var jwt = _config.GetSection("Jwt");
        return (jwt["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key"),
                jwt["Issuer"], jwt["Audience"],
                int.TryParse(jwt["ExpireMinutes"], out var minutes) ? minutes : 30);
    }

    private static SigningCredentials CreateSigningCredentials(string key) =>
        new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

    private static List<Claim> CreateClaims(User user) => [
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Role, user.Role)
    ];
}
