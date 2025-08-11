using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vulyk.Controllers;
using Vulyk.Services;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class CompleteProfileModel : PageModel
    {
        private readonly IUserService _userService;
        public CompleteProfileModel(IUserService userService)
        {
            _userService = userService;
        }
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            [Required]
            [StringLength(20, MinimumLength = 2, ErrorMessage = "The full name length needs to be from 2 to 20 characters")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _userService.EditFullNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, Input.FullName);
            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }
    }
}
