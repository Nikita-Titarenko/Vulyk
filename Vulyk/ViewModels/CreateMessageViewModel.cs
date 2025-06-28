using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class CreateMessageViewModel
    {
        public int UserId { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
    }
}