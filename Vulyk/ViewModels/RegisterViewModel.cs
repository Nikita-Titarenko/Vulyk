using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.ViewModels
{
    public class RegisterViewModel : LoginViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The full name length needs to be from 2 to 20 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
    }
}