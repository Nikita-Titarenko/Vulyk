using System.Security.Claims;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Cmp;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Filters;
using Vulyk.Models;
using Vulyk.Services;
using Vulyk.ViewModels;

namespace Vulyk.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IUserService userService, IMapper mapper, SignInManager<ApplicationUser> signInManager)
        {
            _userService = userService;
            _mapper = mapper;
            _signInManager = signInManager;
        }

        [DenyAuthenticatedAttribute]
        public IActionResult Register()
        {
            return View();
        }

        [DenyAuthenticatedAttribute]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registrationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationViewModel);
            }

            var result = await _userService.AddUserAsync(_mapper.Map<RegistrationDto>(registrationViewModel));

            if (result == UserService.AddUserResult.EmailAlreadyExist)
            {
                ModelState.AddModelError(string.Empty, "Email already exist");
                return View(registrationViewModel);
            }

            return RedirectToAction(nameof(VerifyEmail), "Account", new { registrationViewModel.Email });
        }

        public IActionResult ExternalLogin(string provider, string? returnUrl)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }
            var result = await _userService.ProcessExternalLoginAsync(info);
            if (result == UserService.GoogleLoginResult.Error)
            {
                return RedirectToAction(nameof(Login), "Account");
            }
            if (result == UserService.GoogleLoginResult.Register)
            {
                return RedirectToAction(nameof(FullNameInput), "Account");
            }
            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }

        [DenyAuthenticatedAttribute]
        public IActionResult VerifyEmail(string email, bool? tokenIncorrect)
        {
            if (tokenIncorrect != null)
            {
                ModelState.AddModelError(string.Empty, "Your email could not be verified");
            }
            return View(new EmailViewModel {Email = email });
        }

        [DenyAuthenticatedAttribute]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            bool isVerified = await _userService.CheckVerificationTokenAsync(new EmailConfirmDto { UserId = userId, VerificationToken = token });
            if (!isVerified)
            {
                return RedirectToAction(nameof(VerifyEmail), "Account", new {email = await _userService.GetEmailAsync(userId), tokenIncorrect = true});
            }
            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }

        [Authorize]
        public IActionResult FullNameInput(string? fullName)
        {
            return View(new FullNameViewModel {FullName = fullName ?? string.Empty });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FullNameInput(FullNameViewModel fullNameViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(fullNameViewModel);
            }

            await _userService.EditFullNameAsync(GetUserId()!, fullNameViewModel.FullName);

            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }

        [DenyAuthenticatedAttribute]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [DenyAuthenticatedAttribute]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel emailInput)
        {
            if (!ModelState.IsValid)
            {
                return View(emailInput);
            }

            var result = await _userService.LoginAsync(emailInput.Email, emailInput.Password);
            if (result == UserService.FindUserResult.EmailEntered)
            {
                return RedirectToAction(nameof(ConfirmEmail), "Account", new { emailInput.Email });
            }
            if (result == UserService.FindUserResult.NotFound)
            {
                ModelState.AddModelError(string.Empty, "Email or password incorrect");
                return View(emailInput);
            }

            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _userService.LogOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
