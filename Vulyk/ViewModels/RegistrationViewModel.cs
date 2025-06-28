using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class RegistrationViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
