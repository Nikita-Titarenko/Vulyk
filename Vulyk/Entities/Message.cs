using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.Entities
{
    public class Message
    {
        public int Id { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDateTime { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ChatId { get; set; }
        public UserChat UserChat { get; set; } = null!;
    }
}
