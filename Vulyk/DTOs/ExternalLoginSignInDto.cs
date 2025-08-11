namespace Vulyk.DTOs
{
    public class ExternalLoginSignInDto
    {
        public string LoginProvider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
