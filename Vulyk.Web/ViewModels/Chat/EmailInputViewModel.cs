using System.ComponentModel.DataAnnotations;

namespace Vulyk.Web.ViewModels.Chat
{
    public class EmailInputViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}