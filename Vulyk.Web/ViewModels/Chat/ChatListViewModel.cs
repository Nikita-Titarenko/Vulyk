namespace Vulyk.Web.ViewModels.Chat
{
    public class ChatListViewModel
    {
        public List<ChatListItemViewModel> ChatItems = new List<ChatListItemViewModel>();
        public string UserId { get; set; } = string.Empty;
        public string? NewUserId { get; set; } = string.Empty;
        public int? DisplayChatId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}