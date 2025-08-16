using AutoMapper;
using Vulyk.DTOs;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : AnonymousOnlyModel
    {
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public ForgotPasswordConfirmation(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _userService.GeneratePasswordResetTokenAsync(Email);
            if (result.IsSuccess)
            {
                var callbackUrl = CreateEmailConfirmationLink(result.Value, "/Account/ResetPassword", null);

                await _emailSender.SendEmailAsync(
                    Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }


            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
