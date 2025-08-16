namespace Vulyk.DTOs.Account
{
    public class AuthResultDto
    {
        public bool EmailNotConfirmed { get; set; }
        public bool PasswordNotExist { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
