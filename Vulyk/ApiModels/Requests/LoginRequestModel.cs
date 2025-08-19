using Vulyk.Filters;

namespace Vulyk.ApiModels.Requests
{
    public class LoginRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
    }
}
