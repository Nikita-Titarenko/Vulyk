using System.ComponentModel.DataAnnotations;
using Vulyk.Filters;

namespace Vulyk.ViewModels
{
    public class EditPasswordViewModel : AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The password length needs to be from 6 characters")]
        [StrongPassword]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}