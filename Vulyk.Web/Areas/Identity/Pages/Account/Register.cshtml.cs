using AutoMapper;
using Vulyk.Application.DTOs.Account;
using Vulyk.Application.Services.User;
using Vulyk.Infrastructure.Models;
using Vulyk.Web.Filters;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : AnonymousOnlyModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(IUserService userService, IMapper mapper, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment) : base(userService, mapper, emailSender)
        {
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
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
            var emailTemplatePath = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "email_layout.html");
            var template = await System.IO.File.ReadAllTextAsync(emailTemplatePath);
            var messageBody = template
                .Replace("{FullName}", Input.FullName)
                .Replace("{MessageContent}", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                messageBody);

            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
        }
    }
}
