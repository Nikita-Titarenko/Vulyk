using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class ChatListViewModel
    {
        public List<ChatListItemViewModel> ChatItemsViewModels = new List<ChatListItemViewModel>();
        public int UserId { get; set; }
        public int? NewUserId { get; set; }
        public int? DisplayChatId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}