using AutoMapper;
using Vulyk.Application.Services.User;
using Vulyk.Infrastructure.Models;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    public class ExternalLoginModel : AnonymousOnlyModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalLoginModel(IUserService userService, IMapper mapper, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager) : base(userService, mapper, emailSender)
        {
            _signInManager = signInManager;
        }

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnGet(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["prompt"] = "consent select_account";
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? "~/Chat/Index";
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var result = await _userService.ProcessExternalLoginAsync(info);
            if (!result.Value.IsLogin)
            {
                return RedirectToPage("CompleteProfile");
            }

            return LocalRedirect(Url.Content(returnUrl));
        }
    }
}
