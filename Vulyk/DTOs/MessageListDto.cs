using Vulyk.ViewModels;

namespace Vulyk.DTOs
{
    public class MessageListDto
    {
        public int ChatId { get; set; }

        public List<MessageListItemDto> Messages = new List<MessageListItemDto>();

        public string UserName = string.Empty;
    }
}
