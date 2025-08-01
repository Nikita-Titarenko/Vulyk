using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class VerificationCodeConfirmViewModel : BaseEmailViewModel
    {
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The verification code length needs to be 6 characters")]
        public string VerificationCode { get; set; } = string.Empty;
    }
}