namespace Vulyk.Application.DTOs.Message
{
    public class CreateMessageDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string PartnerId { get; set; } = string.Empty;
    }
}
