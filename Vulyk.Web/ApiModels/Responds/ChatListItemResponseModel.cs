namespace Vulyk.Web.ViewModels.Chat
{
    public class ChatListItemResponseModel
    {
        public int ChatId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string FullName = string.Empty;

        public DateTime? LastMessageDateTime { get; set; }

        public string? LastMessageText { get; set; }
    }
}