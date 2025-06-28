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
        private readonly UserService _userService;
        public ProfileController(UserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> EditProfile()
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UserEditDto? user = await _userService.FindUserAsync(userId.Value);
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

            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Dictionary<string, string> errors = await _userService.CheckUniqueColumnsAsync(userId.Value, null, editProfileViewModel.Email, editProfileViewModel.Phone);
            if (errors.Count != 0)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(editProfileViewModel);
            }

            UserEditDto? user = await _userService.FindUserAsync(userId.Value);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            user.Email = editProfileViewModel.Email;
            user.Phone = editProfileViewModel.Phone;
            user.Name = editProfileViewModel.Name;
            await _userService.EditUserAsync(userId.Value, user);
            ViewBag.SuccessMessage = "Credentials successful changed!";
            return View(editProfileViewModel);
        }
    }
}
