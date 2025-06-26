using Vulyk.Models;

namespace Vulyk.ViewModels
{
    public class MessageListViewModel
    {
        public int? ChatId { get; set; }
        public int UserId { get; set; }

        public List<MessageListItemViewModel> Messages = new List<MessageListItemViewModel>();

        public string UserName = string.Empty;
    }
}
