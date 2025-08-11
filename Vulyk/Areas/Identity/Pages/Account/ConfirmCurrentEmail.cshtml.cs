using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Vulyk.Controllers;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class ConfirmCurrentEmailModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public ConfirmCurrentEmailModel(IUserService userService, IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public string UserId { get; set; } = string.Empty;

        [BindProperty]
        public string Code { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _userService.ConfirmCurrentEmailAsync(new DTOs.ConfirmTokenDto { UserId = userId, Code = code }, null);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                StatusMessage = "Error confirming your email.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UserId == null || Code == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _userService.ConfirmCurrentEmailAsync(new DTOs.ConfirmTokenDto { UserId = UserId, Code = Code }, Input.Email);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                StatusMessage = "Error confirming your email.";
                return Page();
            }
            result = await _userService.GenerateNewEmailConfirmationTokenAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var callbackUrl = Url.Page(
    "/Account/ConfirmNewEmail",
    pageHandler: null,
    values: new { area = "Identity", userId = result.Value.UserId, code = result.Value.Code },
    protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email for change email",
                $"Please confirm email changing by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("NewEmailConfirmation");
        }
    }
}
