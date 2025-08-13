namespace Vulyk.DTOs
{
    public class GetMessagesDto
    {
        public string UserId { get; set; } = string.Empty;
        public int ChatId { get; set; }
        public string PartnerId { get; set; } = string.Empty;
    }
}