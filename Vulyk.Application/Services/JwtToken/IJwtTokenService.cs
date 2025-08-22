namespace Vulyk.Application.Services.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(string userId);
    }
}