using AutoMapper;
using Vulyk.DTOs;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : BaseManagePageModel
    {
        public IndexModel(IUserService userService, IEmailSender emailSender, IMapper mapper) : base(userService, mapper, emailSender) { }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [BindProperty]
        public bool IsPasswordExist { get; set; }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [StringLength(20, MinimumLength = 2, ErrorMessage = "The full name length needs to be from 2 to 20 characters")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone number")]
            [StringLength(20, ErrorMessage = "The password length needs to be from 2 to 20 characters")]
            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            var getUserProfileResult = await _userService.GetUserProfileAsync(GetUserId());
            if (!getUserProfileResult.IsSuccess)
            {
                foreach (var error in getUserProfileResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return Page();
            }

            Input = _mapper.Map<InputModel>(getUserProfileResult.Value);
            Email = getUserProfileResult.Value.Email;
            IsPasswordExist = getUserProfileResult.Value.IsPasswordExist;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            var dto = _mapper.Map<UserProfileEditDto>(Input);
            dto.UserId = GetUserId();
            var result = await _userService.EditUserProfileAsync(dto);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRequestCurrentEmailConfirmation()
        {
            var result = await _userService.GenerateCurrentEmailConfirmationTokenByIdAsync(GetUserId());
            var callbackUrl = CreateEmailConfirmationLink(result.Value, "/Account/ConfirmCurrentEmail", null);

            await _emailSender.SendEmailAsync(Email, "Confirm your email for change email",
                $"Please confirm email changing by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("/Account/Manage/CurrentEmailConfirmation", new { Email });
        }
    }
}
