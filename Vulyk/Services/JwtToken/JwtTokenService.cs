using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vulyk.Settings;

namespace Vulyk.Services.JwtToken
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IOptions<JwtSettings> _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwtToken(string userId)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Audience = _jwtSettings.Value.Audience,
                Issuer = _jwtSettings.Value.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret)), SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
