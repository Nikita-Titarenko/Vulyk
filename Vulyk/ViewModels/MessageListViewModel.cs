using System.ComponentModel.DataAnnotations;
using Vulyk.Models;

namespace Vulyk.ViewModels
{
    public class MessageListViewModel
    {
        public int? ChatId { get; set; }
        public int UserId { get; set; }

        public List<MessageListItemViewModel> Messages { get; set; } = new List<MessageListItemViewModel>();
        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}