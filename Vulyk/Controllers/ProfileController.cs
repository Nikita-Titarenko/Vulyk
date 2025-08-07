using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vulyk.Controllers;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.Services;
using Vulyk.ViewModels;
using static Vulyk.Services.UserService;

namespace Vulyk.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public ProfileController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            string? userId = GetUserId()!;

            UserProfileEditDto? user = await _userService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            EditProfileViewModel editProfileViewModel = _mapper.Map<EditProfileViewModel>(user);

            return View(editProfileViewModel);
        }

        [Authorize]
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

            string? userId = GetUserId()!;

            UserProfileEditDto? user = _mapper.Map<UserProfileEditDto>(editProfileViewModel);

            await _userService.EditUserProfileAsync(userId, user);
            ViewBag.SuccessMessage = "Credentials successful changed!";
            return View(editProfileViewModel);
        }

        [Authorize]
        public IActionResult AddPassword()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string? userId = GetUserId()!;

            UserProfileEditDto? user = await _userService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            await _userService.AddPasswordAsync(userId, model.NewPassword, model.NewPasswordConfirm);
            ViewBag.SuccessMessage = "Password successful added!";

            return View();
        }

        [Authorize]
        public IActionResult EditPassword()
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel model)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string? userId = GetUserId()!;

            UserProfileEditDto? user = await _userService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            var dto = _mapper.Map<EditPasswordByCurrentPasswordDto>(model);
            dto.UserId = userId;
            var result = await _userService.EditPasswordByCurrentPasswordAsync(dto);
            if (result == UserService.EditPasswordResult.CurrentPasswordIncorrect)
            {
                ModelState.AddModelError(string.Empty, "Current password incorrect");
                return View(model);
            }
            ViewBag.SuccessMessage = "Password successful changed!";

            return View();
        }

        [Authorize]
        public IActionResult EditEmail(string email)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;

            return RedirectToAction(nameof(AccountController.SendVerificationToken),
                "Account",
                new
                {
                    email,
                    returnUrl = "/Profile/NewEmailInput",
                    emailConfirmation = EmailConfirmation.ConfirmCurrentEmail
                }
             );
        }

        [Authorize]
        public IActionResult NewEmailInput(string? token)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;

            return View(new NewEmailConfirmViewModel { VerificationToken = token! });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NewEmailInput(NewEmailConfirmViewModel model)
        {
            ViewData["ChoosedPage"] = "EditProfile";
            ViewData["SidepanelVisibility"] = false;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string userId = GetUserId()!;
            bool result = await _userService.CheckVerificationTokenAsync(
                new EmailConfirmDto
                {
                    UserId = userId,
                    NewEmail = model.NewEmail,
                    Token = model.VerificationToken
                },
                EmailConfirmation.ConfirmCurrentEmail);
            if (!result)
            {
                return RedirectToAction(nameof(AccountController.ConfirmEmail), "Account", new { userId, tokenIncorrect = true});
            }
            await _userService.SendEmailConfirmationTokenAsync(userId, EmailConfirmation.ConfirmNewEmail, "/Profile/EditProfile");
            return RedirectToAction(nameof(AccountController.VerifyEmail), "Account", new { email = model.NewEmail, emailConfirmation = EmailConfirmation.ConfirmNewEmail, token = model.VerificationToken});
        }
    }
}