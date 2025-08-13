using System.ComponentModel.DataAnnotations;
using Vulyk.ViewModels;

namespace Vulyk.DTOs
{
    public class MessageListDto
    {
        public string PartnerId { get; set; } = string.Empty;
        public int ChatId { get; set; }

        public List<MessageListItemDto> Messages { get; set; } = new List<MessageListItemDto>();
        public string FullName { get; set; } = string.Empty;
    }
}