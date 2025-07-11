using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.ViewModels
{
    public class EditProfileViewModel
    {
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
    }
}