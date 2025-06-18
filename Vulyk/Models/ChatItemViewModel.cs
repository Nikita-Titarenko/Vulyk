namespace Vulyk.Models
{
    public class ChatItemViewModel
    {
        public string Name = string.Empty;

        public DateTime LastMessageDateTime { get; set; }

        public string LastMessageText { get; set; } = string.Empty;
    }
}
