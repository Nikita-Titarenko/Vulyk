using System.ComponentModel.DataAnnotations;

namespace Vulyk.DTOs
{
    public class ChatPartnerDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime LastOnline { get; set; }
    }
}
