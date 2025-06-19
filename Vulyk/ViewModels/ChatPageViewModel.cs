namespace Vulyk.ViewModels
{
    public class ChatPageViewModel
    {
        public List<ChatListItemViewModel> chatItemViewModels = new List<ChatListItemViewModel>();
        public List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
        public int? CurrentChatId {  get; set; }
    }
}
