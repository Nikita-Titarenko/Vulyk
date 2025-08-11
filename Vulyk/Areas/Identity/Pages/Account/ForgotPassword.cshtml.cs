using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;

        public ForgotPasswordModel(IEmailSender emailSender, IUserService userService)
        {
            _emailSender = emailSender;
            _userService = userService;
        }

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
                var callbackUrl = Url.Page(
    "/Account/ResetPassword",
    pageHandler: null,
    values: new { area = "Identity", code = result.Value.Code, userId = result.Value.UserId },
    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }


            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
