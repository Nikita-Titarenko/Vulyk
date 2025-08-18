using Microsoft.Extensions.Options;
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
            return "";
        }
    }
}
