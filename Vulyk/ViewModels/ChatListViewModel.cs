namespace Vulyk.ViewModels
{
    public class ChatListViewModel
    {
        public List<ChatListItemViewModel> chatItemViewModels = new List<ChatListItemViewModel>();
        public int? CurrentChatIndex {  get; set; }
    }
}
