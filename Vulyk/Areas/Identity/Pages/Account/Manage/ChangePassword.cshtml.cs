using System.Security.Claims;
using AutoMapper;
using Vulyk.DTOs.Profile;
using Vulyk.Filters;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : BaseManagePageModel
    {
        public ChangePasswordModel(IUserService userService, IEmailSender emailSender, IMapper mapper) : base(userService, mapper, emailSender) { }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public class InputModel
        {

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            [StrongPassword]
            public string OldPassword { get; set; } = string.Empty;

            [Required]
            [StrongPassword]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var hasPassword = await _userService.HasPasswordAsync(GetUserId());
            if (!hasPassword.IsSuccess)
            {
                return NotFound($"Unable to load user with ID '{User.FindFirstValue(ClaimTypes.NameIdentifier)}'.");
            }

            if (!hasPassword.Value)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var dto = _mapper.Map<ChangePasswordDto>(Input);
            dto.UserId = GetUserId();
            var result = await _userService.ChangePasswordAsync(dto);

            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                return Page();
            }

            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}
