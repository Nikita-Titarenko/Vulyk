using AutoMapper;
using Vulyk.Application.DTOs.Account;
using Vulyk.Application.Services.User;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    public class RegisterConfirmationModel : AnonymousOnlyModel
    {
        public RegisterConfirmationModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string ReturnUrl { get; set; } = string.Empty;

        public IActionResult OnGet(string email, string? returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            ReturnUrl = returnUrl ?? Url.Content("~/Chat/Index");

            Email = email;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _userService.GenerateCurrentEmailConfirmationTokenByEmailAsync(Email);

            var callbackUrl = CreateEmailConfirmationLink(_mapper.Map<ConfirmTokenDto>(result.Value), "/Account/ConfirmEmail", ReturnUrl);

            await _emailSender.SendEmailAsync(Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Page();
        }
    }
}
