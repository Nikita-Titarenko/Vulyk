namespace Vulyk.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDateTime { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public UserChat UserChat { get; set; } = null!;
    }
}
