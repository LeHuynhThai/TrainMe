using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private DateTime _expires;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateAccessToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var jwt = _config.GetSection("Jwt");

            var key = jwt["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var minutes = int.TryParse(jwt["ExpireMinutes"], out var m) ? m : 30;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            _expires = DateTime.UtcNow.AddMinutes(minutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: _expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetExpiration() => _expires;
    }
}
