using AutoMapper;
using Vulyk.Application.DTOs.Account;
using Vulyk.Application.Services.User;

namespace Vulyk.Web.Areas.Identity.Pages.Account.Manage
{
    public class CurrentEmailConfirmationModel : BaseManagePageModel
    {
        public CurrentEmailConfirmationModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public IActionResult OnGet(string email)
        {
            Email = email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _userService.GenerateCurrentEmailConfirmationTokenByEmailAsync(Email);

            var callbackUrl = CreateEmailConfirmationLink(_mapper.Map<ConfirmTokenDto>(result.Value), "/Account/ConfirmCurrentEmail");

            await _emailSender.SendEmailAsync(Email, "Confirm your email for change email",
                $"Please confirm email changing by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Page();
        }
    }
}