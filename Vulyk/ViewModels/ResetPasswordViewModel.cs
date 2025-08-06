namespace Vulyk.ViewModels
{
    public class ResetPasswordViewModel : AddPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
