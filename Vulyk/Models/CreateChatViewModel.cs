using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace Vulyk.Models
{
    public class CreateChatViewModel : IValidatableObject
    {
        [Display(Name = "Your login")]
        public string Login { get; set; } = string.Empty;
        [Display(Name = "Login")]
        public string? LoginToAdd { get; set; } = string.Empty;
        [Display(Name = "Phone")]
        public string? PhoneToAdd { get; set; } = string.Empty;
        public CreateType CreateType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CreateType.Equals(CreateType.Login) && LoginToAdd.IsNullOrEmpty())
            {
                yield return new ValidationResult("Login is require", [nameof(LoginToAdd)]);
            }
            if (CreateType.Equals(CreateType.Phone) && PhoneToAdd.IsNullOrEmpty())
            {
                yield return new ValidationResult("Phone is require", [nameof(PhoneToAdd)]);
            }
        }
    }

    public enum CreateType
    {
        Login, Phone
    }
}
