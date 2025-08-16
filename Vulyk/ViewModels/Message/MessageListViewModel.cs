using System.ComponentModel.DataAnnotations;
using Vulyk.Models;

namespace Vulyk.ViewModels.Message
{
    public class MessageListViewModel
    {
        public int? ChatId { get; set; }
        public string PartnerId { get; set; } = string.Empty;
        public List<MessageListItemViewModel> Messages { get; set; } = new List<MessageListItemViewModel>();
        public string FullName { get; set; } = string.Empty;
    }
}