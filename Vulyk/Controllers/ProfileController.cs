using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.Controllers;
using Vulyk.Services;
using Vulyk.ViewModels;
using Vulyk.Models;
using Vulyk.DTOs;

namespace Vulyk.Controllers
{
    public class ProfileController : BaseController
    {
        private ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> EditProfile()
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            UserService userService = new UserService(_context);
            UserEditDto? user = await userService.FindUserAsync(userId.Value);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            EditProfileViewModel editProfileViewModel = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone
            };

            return View(editProfileViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editProfileViewModel);
            }
            UserService userService = new UserService(_context);
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            Dictionary<string, string> errors = await userService.CheckUniqueColumnsAsync(userId.Value, null, editProfileViewModel.Email, editProfileViewModel.Phone);
            if (errors.Count != 0)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(editProfileViewModel);
            }

            UserEditDto? user = await userService.FindUserAsync(userId.Value);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            user.Email = editProfileViewModel.Email;
            user.Phone = editProfileViewModel.Phone;
            user.Name = editProfileViewModel.Name;
            await userService.EditUserAsync(userId.Value, user);
            ViewBag.SuccessMessage = "Credentials successful changed!";
            return View(editProfileViewModel);
        }
    }
}
