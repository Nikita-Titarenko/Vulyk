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
            ViewData["ChoosedPage"] = "EditProfile";
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
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Password = user.Password,
            };

            return View(editProfileViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editProfileViewModel)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            if (!ModelState.IsValid)
            {
                return View(editProfileViewModel);
            }

            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UserEditDto? user = new UserEditDto
            {
                Email = editProfileViewModel.Email,
                Phone = editProfileViewModel.Phone,
                FullName = editProfileViewModel.FullName,
                Password = editProfileViewModel.Password,
            };

            await _userService.EditUserAsync(userId.Value, user);
            ViewBag.SuccessMessage = "Credentials successful changed!";
            return View(editProfileViewModel);
        }
    }
}
