using Vulyk.Web.Filters;

namespace Vulyk.Web.ApiModels.Requests
{
    public class RegisterRequestModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The Full Name must be at max 20 characters long.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [StrongPassword]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
