namespace Vulyk.DTOs
{
    public class MessageListItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDateTime { get; set; }
        public bool IsMine {  get; set; }
    }
}