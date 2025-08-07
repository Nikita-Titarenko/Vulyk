using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Ocsp;
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
        public async Task<IActionResult> Register(RegisterViewModel registrationViewModel, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationViewModel);
            }

            var result = await _userService.AddUserAsync(_mapper.Map<RegistrationDto>(registrationViewModel), returnUrl);

            if (result == UserService.AddUserResult.EmailAlreadyExist)
            {
                ModelState.AddModelError(string.Empty, "Email already exist");
                return View(registrationViewModel);
            }

            if (result == UserService.AddUserResult.PasswordTooWeak)
            {
                ModelState.AddModelError(string.Empty, "Email does not meet the requiements");
                return View(registrationViewModel);
            }

            return RedirectToAction(nameof(VerifyEmail), "Account", new { registrationViewModel.Email, emailConfirmation = UserService.EmailConfirmation.ConfirmRegister, returnUrl });
        }

        public IActionResult ExternalLogin(string provider, string? returnUrl)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["prompt"] = "consent select_account";
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl)
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
            if (returnUrl == null)
            {
                return RedirectToAction(nameof(ChatController.Index), "Chat");
            }
            return Redirect(returnUrl);
        }

        public IActionResult InputWrongEmail(UserService.EmailConfirmation emailConfirmation, string? returnUrl, string? token)
        {
            if (emailConfirmation == UserService.EmailConfirmation.ConfirmCurrentEmail)
            {
                return RedirectToAction(nameof(ProfileController.EditProfile), "Profile");
            }

            if (emailConfirmation == UserService.EmailConfirmation.ConfirmNewEmail)
            {
                return RedirectToAction(nameof(ProfileController.NewEmailInput), "Profile", new { token });
            }

            if (emailConfirmation == UserService.EmailConfirmation.ConfirmLogin)
            {
                return RedirectToAction(nameof(Login), "Account", new {returnUrl});
            }

            if (emailConfirmation == UserService.EmailConfirmation.ConfirmRegister)
            {
                return RedirectToAction(nameof(Register), "Account", new { returnUrl });
            }

            if (emailConfirmation == UserService.EmailConfirmation.ResetPassword)
            {
                return RedirectToAction(nameof(ForgotPassword), "Account", new { returnUrl });
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public async Task<IActionResult> SendVerificationToken(UserService.EmailConfirmation emailConfirmation, string email, string? token, string? returnUrl)
        {
            await _userService.SendEmailConfirmationTokenAsync(email, emailConfirmation);
            return RedirectToAction(nameof(VerifyEmail), "Account", new { email, emailConfirmation, token, returnUrl });
        }

        public IActionResult VerifyEmail(VerifyEmailViewModel model)
        {
            return View(model);
        }

        //[DenyAuthenticatedAttribute]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string? returnUrl, UserService.EmailConfirmation emailConfirmation)
        {
            bool isVerified = await _userService.CheckVerificationTokenAsync(new EmailConfirmDto { UserId = userId, Token = token }, emailConfirmation);
            string? email;
            if (emailConfirmation == UserService.EmailConfirmation.ConfirmNewEmail)
            {
                email = await _userService.GetPendingNewEmailAsync(userId);
            } else
            {
                email = await _userService.GetEmailAsync(userId);
            }
            if (!isVerified)
            {
                return RedirectToAction(nameof(VerifyEmail), "Account", new { email, emailConfirmation, tokenIncorrect = true, returnUrl });
            }
            if (returnUrl == null)
            {
                return RedirectToAction(nameof(ChatController.Index), "Chat");
            }
            return Redirect(returnUrl);
        }

        [Authorize]
        public IActionResult FullNameInput(string? fullName)
        {
            return View(new FullNameViewModel {FullName = fullName ?? string.Empty });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FullNameInput(FullNameViewModel fullNameViewModel, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(fullNameViewModel);
            }

            await _userService.EditFullNameAsync(GetUserId()!, fullNameViewModel.FullName);

            if (returnUrl == null)
            {
                return RedirectToAction(nameof(ChatController.Index), "Chat");
            }
            return Redirect(returnUrl);
        }

        [DenyAuthenticatedAttribute]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [DenyAuthenticatedAttribute]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel emailInput, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(emailInput);
            }

            var result = await _userService.LoginAsync(emailInput.Email, emailInput.Password, returnUrl);
            if (result == UserService.FindUserResult.EmailEntered)
            {
                return RedirectToAction(nameof(VerifyEmail), "Account", new { email = emailInput.Email, emailConfirmation = UserService.EmailConfirmation.ConfirmLogin, returnUrl });
            }
            if (result == UserService.FindUserResult.LoginFailed)
            {
                ModelState.AddModelError(string.Empty, "Email or password incorrect");
                return View(emailInput);
            }

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
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

        [DenyAuthenticatedAttribute]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [DenyAuthenticatedAttribute]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailInputViewModel emailViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(emailViewModel);
            }
            await _userService.SendEmailConfirmationTokenAsync(emailViewModel.Email, UserService.EmailConfirmation.ResetPassword);
            return RedirectToAction(nameof(VerifyEmail), "Account", new { emailViewModel.Email, emailConfirmation = UserService.EmailConfirmation.ResetPassword });
        }

        [DenyAuthenticatedAttribute]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordViewModel { UserId = userId, Token = token});
        }

        [DenyAuthenticatedAttribute]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordViewModel);
            }
            var result = await _userService.ResetPasswordAsync(_mapper.Map<ResetPasswordDto>(resetPasswordViewModel));
            if (result == UserService.EditPasswordResult.TokenIncorrect)
            {
                ModelState.AddModelError(string.Empty, "Token is incorrect");
                return View(resetPasswordViewModel);
            }
            return RedirectToAction(nameof(ChatController.Index), "Chat");
        }
    }
}
