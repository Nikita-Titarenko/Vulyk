using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class MessageListItemViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public DateTime CreationDateTime { get; set; }
        public bool IsMine { get; set; }
    }
}
