using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Vulyk.DTOs;
using Vulyk.Filters;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : BaseManagePageModel
    {
        public SetPasswordModel(IUserService userService, IEmailSender emailSender, IMapper mapper) : base(userService, mapper, emailSender) { }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public class InputModel
        {
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

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var dto = _mapper.Map<SetPasswordDto>(Input);
            dto.UserId = GetUserId();
            var result = await _userService.SetPasswordAsync(dto);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return Page();
            }
            return RedirectToPage("/Account/Manage/Index");
        }
    }
}
