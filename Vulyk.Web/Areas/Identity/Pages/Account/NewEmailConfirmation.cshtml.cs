using AutoMapper;
using Vulyk.Application.Services.User;
using Vulyk.Web.Areas.Identity.Pages.Account.Manage;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    public class NewEmailConfirmationModel : BaseManagePageModel
    {
        public NewEmailConfirmationModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string UserId { get; set; } = string.Empty;

        [BindProperty]
        public string Code { get; set; } = string.Empty;

        public void OnGet(string email, string userId, string code)
        {
            Email = email;
            UserId = userId;
            Code = code;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _userService.GenerateNewEmailConfirmationTokenAsync(UserId);
            var callbackUrl = CreateEmailConfirmationLink(result.Value, "/Account/ConfirmNewEmail", null);

            await _emailSender.SendEmailAsync(Email, "Confirm your email for change email",
                $"Please confirm email changing by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Page();
        }
    }
}
