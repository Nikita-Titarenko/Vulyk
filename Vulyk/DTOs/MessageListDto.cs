using System.ComponentModel.DataAnnotations;
using Vulyk.ViewModels;

namespace Vulyk.DTOs
{
    public class MessageListDto
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }

        public List<MessageListItemDto> Messages { get; set; } = new List<MessageListItemDto>();
        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}