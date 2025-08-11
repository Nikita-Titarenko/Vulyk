using AutoMapper;
using FluentResults;
using Vulyk.Controllers;
using Vulyk.DTOs;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public LoginModel(SignInManager<ApplicationUser> signInManager, IUserService userService,
            IMapper mapper,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userService = userService;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/Chat/Index");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        private string CreateEmailConfirmationLink(AuthResultDto authResultDto, string redirectPage, string? returnUrl = null)
        {
            return Url.Page(
redirectPage,
pageHandler: null,
values: new { area = "Identity", userId = authResultDto.UserId, code = authResultDto.Code, returnUrl = returnUrl },
protocol: Request.Scheme)!;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Chat/Index");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _userService.LoginAsync(_mapper.Map<LoginDto>(Input));
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            if (result.Value.PasswordNotExist)
            {
                var callbackUrl = CreateEmailConfirmationLink(result.Value, "/Account/ResetPassword", returnUrl);

                await _emailSender.SendEmailAsync(Input.Email, "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("ForgotPasswordConfirmation", new { email = Input.Email, returnUrl = returnUrl });
            }

            if (result.Value.EmailNotConfirmed)
            {
                var callbackUrl = CreateEmailConfirmationLink(result.Value, "/Account/ConfirmEmail", returnUrl);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
            }

            return LocalRedirect(returnUrl);
        }
    }
}