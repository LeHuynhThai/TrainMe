using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth;

/// <summary>
/// Service tạo và cung cấp token JWT cùng thời điểm hết hạn.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private DateTime _expires;

    /// <summary>
    /// Khởi tạo TokenService với cấu hình ứng dụng.
    /// </summary>
    /// <param name="config">IConfiguration chứa cấu hình Jwt</param>
    public TokenService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Tạo access token JWT cho người dùng.
    /// </summary>
    /// <param name="user">Người dùng</param>
    /// <returns>Chuỗi JWT đã ký</returns>
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

    /// <summary>
    /// Lấy thời điểm hết hạn của token vừa tạo.
    /// </summary>
    public DateTime GetExpiration() => _expires;

    /// <summary>
    /// Đọc cấu hình Jwt từ appsettings: Key, Issuer, Audience, ExpireMinutes.
    /// </summary>
    private (string Key, string? Issuer, string? Audience, int ExpireMinutes) GetJwtConfiguration()
    {
        var jwt = _config.GetSection("Jwt");
        return (jwt["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key"),
                jwt["Issuer"], jwt["Audience"],
                int.TryParse(jwt["ExpireMinutes"], out var minutes) ? minutes : 30);
    }

    /// <summary>
    /// Tạo thông tin ký HMAC-SHA256 với khoá đối xứng.
    /// </summary>
    private static SigningCredentials CreateSigningCredentials(string key) =>
        new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

    /// <summary>
    /// Tạo danh sách claim cho người dùng (sub, nameid, name, role).
    /// </summary>
    private static List<Claim> CreateClaims(User user) => [
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Role, user.Role)
    ];
}
