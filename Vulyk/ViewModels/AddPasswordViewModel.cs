using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Vulyk.Filters;

namespace Vulyk.ViewModels
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The password length needs to be from 6 characters")]
        [StrongPassword]
        [DisplayName("New password")]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The password length needs to be from 6 characters")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        [StrongPassword]
        [DisplayName("New password confirm")]
        public string NewPasswordConfirm { get; set; } = string.Empty;
    }
}