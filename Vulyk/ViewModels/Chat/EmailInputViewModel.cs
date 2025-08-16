using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels.Chat
{
    public class EmailInputViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}