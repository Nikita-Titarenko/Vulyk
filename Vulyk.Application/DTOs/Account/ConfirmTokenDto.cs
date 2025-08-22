namespace Vulyk.Application.DTOs.Account
{
    public class ConfirmTokenDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? NewEmail {  get; set; }
    }
}