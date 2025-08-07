using Vulyk.Services;

namespace Vulyk.ViewModels
{
    public class VerifyEmailViewModel : EmailViewModel
    {
        public UserService.EmailConfirmation EmailConfirmation { get; set; }
        public string? ReturnUrl { get; set; }
        public bool? TokenIncorrect { get; set; }
        public string? Token { get; set; }
    }
}