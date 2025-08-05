using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class CreateMessageViewModel
    {
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Text { get; set; } = string.Empty;
    }
}