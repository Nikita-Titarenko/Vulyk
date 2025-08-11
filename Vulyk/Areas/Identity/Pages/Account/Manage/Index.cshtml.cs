using System.Security.Claims;
using AutoMapper;
using FluentResults;
using Vulyk.DTOs;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public IndexModel(IUserService userService, IMapper mapper, IEmailSender emailSender)
        {
            _userService = userService;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [StringLength(20, MinimumLength = 2, ErrorMessage = "The full name length needs to be from 2 to 20 characters")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;
            [Phone]
            [Display(Name = "Phone number")]
            [StringLength(20, MinimumLength = 10, ErrorMessage = "The phone length needs to be from 10 to 20 characters")]
            public string PhoneNumber { get; set; } = string.Empty;
            public bool IsPasswordExist { get; set; }
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            var profile = await _userService.FindUserByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Input = _mapper.Map<InputModel>(profile);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            await _userService.EditUserProfileAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), _mapper.Map<UserProfileEditDto>(Input));
            return Page();
        }

        public async Task<IActionResult> OnPostRequestCurrentEmailConfirmation()
        {
            var result = await _userService.GenerateCurrentEmailConfirmationTokenAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var callbackUrl = Url.Page(
    "/Account/ConfirmCurrentEmail",
    pageHandler: null,
    values: new { area = "Identity", userId = result.Value.UserId, code = result.Value.Code },
    protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email for change email",
                $"Please confirm email changing by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("/Account/Manage/CurrentEmailConfirmation");
        }
    }
}
