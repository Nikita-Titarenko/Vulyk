using System.ComponentModel.DataAnnotations;

namespace Vulyk.DTOs.Chat
{
    public class ChatListItemDto
    {
        public int ChatId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string FullName = string.Empty;

        public DateTime? LastMessageDateTime { get; set; }

        public string? LastMessageText { get; set; } = string.Empty;
    }
}
