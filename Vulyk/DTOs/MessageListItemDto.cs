using System.ComponentModel.DataAnnotations;

namespace Vulyk.DTOs
{
    public class MessageListItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public DateTime CreationDateTime { get; set; }
        public bool IsMine {  get; set; }
    }
}