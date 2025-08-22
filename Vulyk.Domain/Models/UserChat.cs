namespace Vulyk.Domain.Models
{
    public class UserChat
    {
        public string UserId { get; set; } = string.Empty;
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
    }
}
