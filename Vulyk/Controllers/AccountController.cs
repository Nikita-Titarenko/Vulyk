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

namespace Vulyk.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel registrationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationViewModel);
            }
            Dictionary<string, string> errors = await _userService.CheckUniqueColumnsAsync(null, registrationViewModel.Login, registrationViewModel.Email, registrationViewModel.Phone);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(registrationViewModel);
            }
            UserRegisterDto user = new()
            {
                Name = registrationViewModel.Name,
                Email = registrationViewModel.Email,
                Phone = registrationViewModel.Phone,
                Login = registrationViewModel.Login,
                Password = registrationViewModel.Password
            };
            int userId = await _userService.AddUserAsync(user);

            createCookie(userId.ToString());

            return RedirectToAction("Index", "Chat");
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            int? userId = await _userService.FindUserAsync(user.Login, user.Password);
            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "Incorrect login or password!");
                return View(user);
            }
            createCookie(userId.Value.ToString());
            return RedirectToAction("Index", "Chat");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        private async void createCookie(string userId)
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
