using AutoMapper;
using Vulyk.Filters;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : AnonymousOnlyModel
    {
        public ForgotPasswordModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _userService.GeneratePasswordResetTokenAsync(Input.Email);
            if (result.IsSuccess)
            {
                var callbackUrl = CreateEmailConfirmationLink(result.Value, "/Account/ResetPassword", null);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }


            return RedirectToPage("./ForgotPasswordConfirmation", new { Input.Email });
        }
    }
}
