using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.Controllers;
using Vulyk.Services;
using Vulyk.ViewModels;
using Vulyk.Models;
using Vulyk.DTOs;
using AutoMapper;

namespace Vulyk.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public ProfileController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<IActionResult> EditProfile()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            UserEditDto? user = await _userService.FindUserAsync(userId.Value);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            EditProfileViewModel editProfileViewModel = _mapper.Map<EditProfileViewModel>(user);

            return View(editProfileViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editProfileViewModel)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            if (!ModelState.IsValid)
            {
                return View(editProfileViewModel);
            }

            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            UserEditDto? user = _mapper.Map<UserEditDto>(editProfileViewModel);

            await _userService.EditUserAsync(userId.Value, user);
            ViewBag.SuccessMessage = "Credentials successful changed!";
            return View(editProfileViewModel);
        }
    }
}