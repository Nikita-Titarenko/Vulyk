using System.Security.Claims;
using AutoMapper;
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

        private readonly IMapper _mapper;

        public AccountController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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
            EmailInputDto user = _mapper.Map<EmailInputDto>(registrationViewModel);
            AddUserResult addUserResult = await _userService.AddUserAsync(user);
            if (addUserResult == AddUserResult.EmailAlreadyExist)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken");
                return View(registrationViewModel);
            }

            return RedirectToAction(nameof(VerificationCodeConfirm), "Account", new { user.Email });
        }
        [HttpPost]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto googleSignInDto)
        {
            GoogleSignInResultDto? googleSignInResultDto = await _userService.GoogleSignIn(googleSignInDto.IdToken);
            if (googleSignInResultDto == null)
            {
                return RedirectToAction(nameof(RegisterEmail), "Account");
            }
            if (googleSignInResultDto.UserId == null)
            {
                return RedirectToAction(nameof(NameAndPasswordInput), "Account", new { email = googleSignInResultDto.Email, fullName = googleSignInResultDto.FullName});
            }
            CreateCookie(googleSignInResultDto.UserId.Value.ToString());
            return RedirectToAction(nameof(ChatController.Index), "Chat");
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
            bool isVerified = await _userService.CheckVerificationCodeAsync(_mapper.Map<VerificationCodeConfirmDto>(verificationCodeConfirm));
            if (!isVerified)
            {
                ModelState.AddModelError(string.Empty, "Confirmation code incorrect");
                return View(verificationCodeConfirm);
            }
            return RedirectToAction(nameof(NameAndPasswordInput), "Account", new { verificationCodeConfirm.Email });
        }

        public IActionResult NameAndPasswordInput(string email, string? fullName)
        {
            return View(new NameAndPasswordInputViewModel { Email = email, FullName = fullName ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NameAndPasswordInput(NameAndPasswordInputViewModel nameAndPasswordInput)
        {
            if (!ModelState.IsValid)
            {
                return View(nameAndPasswordInput);
            }

            int userId = await _userService.AddNameAndPassword(_mapper.Map<NameAndPasswordInputDto>(nameAndPasswordInput));
            CreateCookie(userId.ToString());
            return RedirectToAction(nameof(ChatController.Index), "Chat");
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
                return RedirectToAction(nameof(VerificationCodeConfirm), "Account", new { emailInput.Email });
            }

            if (result.Item2 == FindUserResult.VerificationCodeConfirmed)
            {
                return RedirectToAction(nameof(NameAndPasswordInput), "Account", new { emailInput.Email });
            }

            return RedirectToAction(nameof(LoginPassword), "Account", new {emailInput.Email});
        }
        public IActionResult LoginPassword(string email)
        {
            return View(new EmailAndPasswordInputViewModel { Email = email});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPassword(EmailAndPasswordInputViewModel passwordInput)
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
            return RedirectToAction(nameof(ChatController.Index), "Chat");
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
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
