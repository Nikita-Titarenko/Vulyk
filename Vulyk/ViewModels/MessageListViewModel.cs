using System.ComponentModel.DataAnnotations;
using Vulyk.Entities;

namespace Vulyk.ViewModels
{
    public class MessageListViewModel
    {
        public int? ChatId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<MessageListItemViewModel> Messages { get; set; } = new List<MessageListItemViewModel>();
        public string FullName { get; set; } = string.Empty;
    }
}