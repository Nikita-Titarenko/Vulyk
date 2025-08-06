using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class EmailConfirmViewModel
    {
        public string VerificationToken { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;
    }
}