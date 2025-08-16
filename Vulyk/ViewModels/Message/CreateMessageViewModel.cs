using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels.Message
{
    public class CreateMessageViewModel
    {
        public string PartnerId { get; set; } = string.Empty;
        [Required]
        public string Text { get; set; } = string.Empty;
    }
}