using Vulyk.Controllers;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly IUserService _userService;

        public ConfirmEmailModel(IUserService userService)
        {
            _userService = userService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _userService.ConfirmEmailAsync(new DTOs.ConfirmTokenDto { UserId = userId, Code = code });
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                StatusMessage = "Error confirming your email.";
                return Page();
            }

            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }
    }
}
