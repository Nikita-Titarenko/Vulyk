using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Vulyk.Filters;

namespace Vulyk.ViewModels
{
    public class LoginViewModel : EmailInputViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The password length needs to be from 6 characters")]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
    }
}