namespace Vulyk.Data
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDateTime { get; set; }
        public int UserChatId { get; set; }
    }
}
