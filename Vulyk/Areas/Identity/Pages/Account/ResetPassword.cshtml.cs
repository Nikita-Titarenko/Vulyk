using AutoMapper;
using Vulyk.DTOs.Account;
using Vulyk.Filters;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : AnonymousOnlyModel
    {
        public ResetPasswordModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        [BindProperty]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string Code { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [StrongPassword]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string userId, string? code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                UserId = userId;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var dto = _mapper.Map<ResetPasswordDto>(Input);
            dto.UserId = UserId;
            dto.Code = Code;
            var result = await _userService.ResetPasswordAsync(dto);
            if (result.IsSuccess || result.Errors.Any(e => e.Metadata.GetValueOrDefault("Code") as string == "UserNotFound"))
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
