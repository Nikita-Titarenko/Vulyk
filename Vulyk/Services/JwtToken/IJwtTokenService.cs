namespace Vulyk.Services.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(string userId);
    }
}