using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.Services;
using Vulyk.ViewModels;
using static Vulyk.Services.UserService;

namespace Vulyk.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        public IActionResult RegisterEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmail(EmailInputViewModel registrationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationViewModel);
            }
            EmailInputDto user = new()
            {
                Email = registrationViewModel.Email,
            };
            AddUserResult addUserResult = await _userService.AddUserAsync(user);
            if (addUserResult == AddUserResult.EmailAlreadyExist)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken");
                return View(registrationViewModel);
            }

            return RedirectToAction("VerificationCodeConfirm", "Account", new { user.Email });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto googleSignInDto)
        {
            _userService.GoogleSignIn(googleSignInDto);
            
        }

        public IActionResult VerificationCodeConfirm(string email)
        {
            return View(new VerificationCodeConfirmViewModel {Email = email });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificationCodeConfirm(VerificationCodeConfirmViewModel verificationCodeConfirm)
        {
            if (!ModelState.IsValid)
            {
                return View(verificationCodeConfirm);
            }
            bool isVerified = await _userService.CheckVerificationCodeAsync(new VerificationCodeConfirmDto
            {
                Email = verificationCodeConfirm.Email,
                VerificationCode = verificationCodeConfirm.VerificationCode
            });
            if (!isVerified)
            {
                ModelState.AddModelError(string.Empty, "Confirmation code incorrect");
                return View(verificationCodeConfirm);
            }
            return RedirectToAction("NameAndPasswordInput", "Account", new { verificationCodeConfirm.Email });
        }

        public IActionResult NameAndPasswordInput(string email)
        {
            return View(new NameAndPasswordInputViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NameAndPasswordInput(NameAndPasswordInputViewModel nameAndPasswordInput)
        {
            if (!ModelState.IsValid)
            {
                return View(nameAndPasswordInput);
            }

            int userId = await _userService.AddNameAndPassword(new NameAndPasswordInputDto
            {
                Email = nameAndPasswordInput.Email,
                Password = nameAndPasswordInput.Password,
                FullName = nameAndPasswordInput.FullName
            });
            CreateCookie(userId.ToString());
            return RedirectToAction("Index", "Chat");
        }

        public IActionResult LoginEmail()
        {
            return View(new EmailInputViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginEmail(EmailInputViewModel emailInput)
        {
            if (!ModelState.IsValid)
            {
                return View(emailInput);
            }

            var result = await _userService.FindUserAsync(emailInput.Email);
            if (result.Item2 == FindUserResult.NotFound)
            {
                ModelState.AddModelError(string.Empty, "Email not registered");
                return View(emailInput);
            }

            if (result.Item2 == FindUserResult.EmailInputted)
            {
                return RedirectToAction("VerificationCodeConfirm", "Account", new { emailInput.Email });
            }

            if (result.Item2 == FindUserResult.VerificationCodeConfirmed)
            {
                return RedirectToAction("NameAndPasswordInput", "Account", new { emailInput.Email });
            }

            return RedirectToAction("LoginPassword", "Account", new {emailInput.Email});
        }
        public IActionResult LoginPassword(string email)
        {
            return View(new PasswordInputViewModel { Email = email});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPassword(PasswordInputViewModel passwordInput)
        {
            if (!ModelState.IsValid)
            {
                return View(passwordInput);
            }

            int? userId = await _userService.FindUserAsync(passwordInput.Email, passwordInput.Password);
            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "Password is incorrect");
                return View(passwordInput);
            }
            CreateCookie(userId.Value.ToString());
            return RedirectToAction("Index", "Chat");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        private async void CreateCookie(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(30),
            };

            await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Index", "Home");
        }
    }
}
