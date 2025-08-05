using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.ViewModels
{
    public class EditProfileViewModel : EmailViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The full name length needs to be from 2 to 20 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 10, ErrorMessage = "The phone length needs to be from 10 to 20 characters")]
        public string? Phone { get; set; } = string.Empty;
        public bool IsPasswordExist { get; set; }
    }
}