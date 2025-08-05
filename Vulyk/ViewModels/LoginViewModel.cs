using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace Vulyk.ViewModels
{
    public class LoginViewModel : EmailViewModel
    {
        [Required]
        [EmailAddress]
        public new string Email { get => base.Email; set => base.Email = value; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The password length needs to be from 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).*$", ErrorMessage = "The password needs to have digit, upper and lower case letters and unique symbols")]
        public string Password { get; set; } = string.Empty;
    }
}