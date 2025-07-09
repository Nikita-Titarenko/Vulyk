using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class NameAndPasswordInputViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}