using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class NameAndPasswordInputViewModel : EmailAndPasswordInputViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The full name length needs to be from 2 to 20 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
    }
}