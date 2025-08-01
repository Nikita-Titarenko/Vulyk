using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class ChatListViewModel
    {
        public List<ChatListItemViewModel> ChatItems = new List<ChatListItemViewModel>();
        public int UserId { get; set; }
        public int? NewUserId { get; set; }
        public int? DisplayChatId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}