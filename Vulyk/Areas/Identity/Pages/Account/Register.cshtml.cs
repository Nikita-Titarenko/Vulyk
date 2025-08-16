using AutoMapper;
using Vulyk.DTOs.Account;
using Vulyk.Filters;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class RegisterModel : AnonymousOnlyModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(IUserService userService, IMapper mapper, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager) : base(userService, mapper, emailSender) {
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; } = string.Empty;

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required]
            [StringLength(20, ErrorMessage = "The Full Name must be at max 20 characters long.")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;


            [Required]
            [StrongPassword]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }


        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ??= Url.Content("~/Chat/Index");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/Chat/Index");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _userService.RegisterAsync(_mapper.Map<RegisterDto>(Input));

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken");
                return Page();
            }

            var callbackUrl = CreateEmailConfirmationLink(_mapper.Map<ConfirmTokenDto>(result.Value), "/Account/ConfirmEmail", ReturnUrl);

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
        }
    }
}
